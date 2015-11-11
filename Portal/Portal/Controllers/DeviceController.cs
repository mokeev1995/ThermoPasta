using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Schema;
using Microsoft.AspNet.Identity;
using Portal.DAL;
using Portal.Models.CodeFirstModels;
using Portal.Models.ViewModels;

namespace Portal.Controllers
{
    public class DeviceController : Controller
    {
        private readonly IUnitOfWork _uow;
        public DeviceController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        // GET: Device
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var userData = _uow.UserDataRepository.GetById(userId);

            if (userData != null)
            {
                var devices = userData.Devices.ToList();
                var devicesView = new List<DeviceView>();
                foreach (var device in devices)
                {
                    var newDeviceView = new DeviceView
                    {
                        Id = device.Id,
                        CurrentTemparature = device.CurrentTemparature,
                        Profile = device.Profile.Title,
                        Title = device.Title,
                    };

                    if (!device.Profile.Intervals.Any(i => i.Start <= newDeviceView.CurrentTemparature && newDeviceView.CurrentTemparature <= i.End))
                    {
                        newDeviceView.Status = "no";
                    }
                    else
                    {
                        newDeviceView.Status = device.Profile.Intervals.First(i => i.Start <= newDeviceView.CurrentTemparature && newDeviceView.CurrentTemparature <= i.End).Description;
                    }

                    devicesView.Add(newDeviceView);
                }
                return View(devicesView);
            }
            else
            {
                return RedirectToAction("ChangeUserData", "Manage");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            var profiles = _uow.ProfileRepository.GetAll().Where(p => p.UserDataId == null || p.UserDataId == userId);
            var profilesList = new SelectList(profiles, "Id", "Title");
            ViewBag.Profiles = profilesList;

            return View();
        }

        [HttpPost]
        public ActionResult Create(DeviceCreate model)
        {
            var userId = User.Identity.GetUserId();
            var profiles = _uow.ProfileRepository.GetAll().Where(p => p.UserDataId == null || p.UserDataId == userId);
            var profilesList = new SelectList(profiles, "Id", "Title");
            ViewBag.Profiles = profilesList;

            if (ModelState.IsValid)
            {
                var checkCodes = _uow.CheckCodeRepository.GetAll().Where(cc => cc.Code == model.Code);

                if (checkCodes.Any())
                {
                    if(checkCodes.First().Time.AddMinutes(15) >= DateTime.Now)
                    {
                        var newDevice = new Device
                        {
                            Title = model.Title,
                            ProfileId = model.ProfileId,
                            Id = _uow.CheckCodeRepository.GetAll().First(cc => cc.Code == model.Code).Id,
                            UserDataId = userId
                        };
                        _uow.DeviceRepository.Insert(newDevice);
                        _uow.CheckCodeRepository.Delete(newDevice.Id);
                        _uow.Save();

                        return RedirectToAction("Index");    
                    }

                    ModelState.AddModelError("", "Time expired");
                }

                ModelState.AddModelError("", "Code is encorrect");
            }

            return View(model);
        }

        [HttpGet]
        public string GetCheckCode(string id)
        {
            if (id != null)
            {
                var random = new Random();
                var newCode = "";
                do
                {
                    newCode = string.Format("{0:0000}", random.Next(0, 10000));
                } while (_uow.CheckCodeRepository.GetAll().Any(cc => cc.Code == newCode));

                var newCheckCode = new CheckCode
                {
                    Code = newCode,
                    Id = id,
                    Time = DateTime.Now
                };
                _uow.CheckCodeRepository.Insert(newCheckCode);
                _uow.Save();

                return newCode;
            }

            return "error";
        }
    }
}
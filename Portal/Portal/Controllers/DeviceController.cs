using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Portal.DAL;
using Portal.Models.CodeFirstModels;
using Portal.Models.ViewModels;

namespace Portal.Controllers
{
    [Authorize]
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
                    if (checkCodes.First().Time.AddMinutes(15) >= DateTime.Now)
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
        public ActionResult Edit(string id)
        {
            var userId = User.Identity.GetUserId();
            var profiles = _uow.ProfileRepository.GetAll().Where(p => p.UserDataId == null || p.UserDataId == userId);
            var profilesList = new SelectList(profiles, "Id", "Title");
            ViewBag.Profiles = profilesList;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var device = _uow.DeviceRepository.GetById(id);


            if (device == null)
            {
                return HttpNotFound();
            }

            var deviceView = new DeviceCreate
            {
                Id = device.Id,
                Title = device.Title,
                ProfileId = device.ProfileId,
                Code = "1234"

            };
            return View(deviceView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DeviceCreate model)
        {
            var userId = User.Identity.GetUserId();
            var profiles = _uow.ProfileRepository.GetAll().Where(p => p.UserDataId == null || p.UserDataId == userId);
            var profilesList = new SelectList(profiles, "Id", "Title");
            ViewBag.Profiles = profilesList;

            if (ModelState.IsValid)
            {
                var device = new Device
                {
                    Id = model.Id,
                    ProfileId = model.ProfileId,
                    Title = model.Title,
                    UserDataId = userId
                };

                _uow.DeviceRepository.Update(device);
                _uow.Save();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var device = _uow.DeviceRepository.GetById(id);
            if (device == null)
            {
                return HttpNotFound();
            }

            var deviceView = new DeviceView
            {
                Id = device.Id,
                Title = device.Title,
                Profile = device.Profile.Title
            };

            return View(deviceView);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            _uow.DeviceRepository.Delete(id);
            _uow.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _uow.Dispose();
            }
            base.Dispose(disposing);
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
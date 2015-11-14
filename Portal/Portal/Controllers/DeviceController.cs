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
                return View();
            }
            else
            {
                return RedirectToAction("ChangeUserData", "Manage");
            }
        }


        public ActionResult List()
        {
            var userId = User.Identity.GetUserId();
            var userData = _uow.UserDataRepository.GetById(userId);

            var deviceViewList = new List<DeviceView>();

            var userDevices = userData.UserDevices;
            foreach (var userDevice in userDevices)
            {
                var device = userDevice.Device;

                var temperature = device.Temperatures.Any() ? device.Temperatures.Last().Value : 0;
                var time = device.Temperatures.Any()
                    ? device.Temperatures.Last().Time.ToShortTimeString()
                    : DateTime.Now.ToShortTimeString();
                var newDeviceView = new DeviceView
                {
                    Id = device.Id,
                    LastTemparature = temperature,
                    Time = time,
                    Profile = userDevice.Profile.Title,
                    Title = userDevice.Title,
                    Period = userDevice.Period
                };

                if (!userDevice.Profile.Intervals.Any(i => i.Start <= newDeviceView.LastTemparature && newDeviceView.LastTemparature <= i.End))
                {
                    newDeviceView.Status = "no";
                }
                else
                {
                    newDeviceView.Status = userDevice.Profile.Intervals.First(i => i.Start <= newDeviceView.LastTemparature && newDeviceView.LastTemparature <= i.End).Description;
                }

                deviceViewList.Add(newDeviceView);

            }

            return PartialView(deviceViewList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            var profiles = _uow.ProfileRepository.GetAll().Where(p => p.UserDataId == null || p.UserDataId == userId);
            var profilesList = new SelectList(profiles, "Id", "Title");
            ViewBag.Profiles = profilesList;

            var periods = new int[60];
            for (int i = 0; i < 60; i++)
            {
                periods[i] = i + 1;
            }
            ViewBag.Periods = periods;
            ViewBag.Period = 1;

            return View(new DeviceCreate
            {
                Period = 1
            });
        }

        [HttpPost]
        public ActionResult Create(DeviceCreate model)
        {
            var userId = User.Identity.GetUserId();
            var userData = _uow.UserDataRepository.GetById(userId);
            var profiles = _uow.ProfileRepository.GetAll().Where(p => p.UserDataId == null || p.UserDataId == userId);
            var profilesList = new SelectList(profiles, "Id", "Title");
            ViewBag.Profiles = profilesList;

            if (ModelState.IsValid)
            {
                var checkCodes = _uow.CheckCodeRepository.GetAll().Where(cc => cc.Code == model.Code);

                if (checkCodes.Any())
                {
                    var checkCode = checkCodes.First();

                    if (checkCode.Time.AddMinutes(15) >= DateTime.Now)
                    {
                        if (userData.UserDevices.All(ud => ud.DeviceId != checkCode.Id))
                        {
                            var userDevice = new UserDevice
                            {
                                DeviceId = checkCode.Id,
                                Title = model.Title,
                                ProfileId = model.ProfileId,
                                UserDataId = userId,
                                Period = model.Period
                            };
                            _uow.UserDeviceRepository.Insert(userDevice);
                            _uow.CheckCodeRepository.Delete(checkCode);
                            _uow.Save();

                            return RedirectToAction("Index");
                        }

                        ModelState.AddModelError("", "You already added this device.");
                    }

                    ModelState.AddModelError("", "Time expired. Please, press button on device again.");
                    _uow.CheckCodeRepository.Delete(model.Code);
                    _uow.Save();
                }

                ModelState.AddModelError("", "Code is encorrect.");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var userId = User.Identity.GetUserId();
            var userData = _uow.UserDataRepository.GetById(userId);
            var userDevice = userData.UserDevices.First(ud => ud.DeviceId == id);
            var profiles = _uow.ProfileRepository.GetAll().Where(p => p.UserDataId == null || p.UserDataId == userId);
            var profilesList = new SelectList(profiles, "Id", "Title");
            ViewBag.Profiles = profilesList;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var periods = new int[60];
            for (int i = 0; i < 60; i++)
            {
                periods[i] = i + 1;
            }
            ViewBag.Periods = periods;
            ViewBag.Period = userDevice.Period;

            var deviceEdit = new DeviceEdit
            {
                Id = id,
                Title = userDevice.Title,
                ProfileId = userDevice.ProfileId

            };
            return View(deviceEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DeviceEdit model)
        {
            var userId = User.Identity.GetUserId();
            var userData = _uow.UserDataRepository.GetById(userId);
            var userDevice = userData.UserDevices.First(ud => ud.DeviceId == model.Id);
            var profiles = _uow.ProfileRepository.GetAll().Where(p => p.UserDataId == null || p.UserDataId == userId);
            var profilesList = new SelectList(profiles, "Id", "Title");
            ViewBag.Profiles = profilesList;

            if (ModelState.IsValid)
            {
                userDevice.ProfileId = model.ProfileId;
                userDevice.Title = model.Title;

                if (userDevice.Period != model.Period)
                {
                    userDevice.Period = model.Period;
                }

                _uow.UserDeviceRepository.Update(userDevice);
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

            var userId = User.Identity.GetUserId();
            var userData = _uow.UserDataRepository.GetById(userId);
            var userDevice = userData.UserDevices.First(ud => ud.DeviceId == id);
            if (userDevice == null)
            {
                return HttpNotFound();
            }

            var deviceView = new DeviceView
            {
                Id = userDevice.DeviceId,
                Title = userDevice.Title,
                Profile = userDevice.Profile.Title
            };

            return View(deviceView);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var userId = User.Identity.GetUserId();
            var userData = _uow.UserDataRepository.GetById(userId);
            var userDevice = userData.UserDevices.First(ud => ud.DeviceId == id);

            _uow.UserDeviceRepository.Delete(userDevice);
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
    }
}
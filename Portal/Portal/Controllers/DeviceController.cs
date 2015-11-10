using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
    }
}
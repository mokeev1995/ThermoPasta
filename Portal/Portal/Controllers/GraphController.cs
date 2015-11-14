using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Portal.DAL;
using Portal.Models.CodeFirstModels;

namespace Portal.Controllers
{
    [Authorize]
    public class GraphController : BaseController
	{
        private readonly IUnitOfWork _uow;
        public GraphController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetTemperatures()
        {
            var userId = User.Identity.GetUserId();
            var userData = _uow.UserDataRepository.GetById(userId);
            var userDevices = userData.UserDevices;
            var deviceTemperatures = new List<object>();
            foreach (var userDevice in userDevices)
            {
                deviceTemperatures.Add(new { name = userDevice.Title, value = GetTemperaturesForDevice(userDevice) });
            }

            return Json(deviceTemperatures, JsonRequestBehavior.AllowGet);
        }

        private object GetTemperaturesForDevice(UserDevice userDevice)
        {
            var deviceTemperatures = new List<object> { new IComparable[] { "Время", "Измеренная", "Прогнозируемая" } };

            var temperatures = userDevice.Device.Temperatures.ToArray();
            List<double> arguments = new List<double>();
            List<double> temperatureValues = new List<double>();
            var extend = new double[] { 11, 12, 13, 14, 15 };
            DateTime time;
            double temperature;
            var count = temperatures.Length <= 11 ? temperatures.Length : 10;
            for (var i = 0; i < count - 1; ++i)
            {
                temperature = temperatures[i].Value;
                time = temperatures[i].Time;
                arguments.Add(i);
                temperatureValues.Add(temperature);
                deviceTemperatures.Add(new IComparable[] { time.ToShortTimeString(), temperature, null });
            }

            temperature = temperatures[count - 1].Value;
            time = temperatures[count - 1].Time;
            arguments.Add(count - 1);
            temperatureValues.Add(temperature);
            deviceTemperatures.Add(new IComparable[] { time.ToShortTimeString(), temperature, temperature });

            var interpolantValue = Interpolant(arguments.ToArray(), temperatureValues.ToArray(), extend);
            for (var i = 0; i < interpolantValue.Length; ++i)
            {
                var time2 = time.AddMinutes(i + 1);
                deviceTemperatures.Add(new IComparable[] { time2.ToShortTimeString(), null, interpolantValue[i] });
            }

            return deviceTemperatures;
        }

        private double[] Interpolant(double[] arguments, double[] values, double[] extendsArguments)
        {
            alglib.spline1dinterpolant line;

            alglib.spline1dbuildlinear(arguments, values, out line);


            var result = new List<double>();
            foreach (var xE in extendsArguments)
            {
                result.Add(alglib.spline1dcalc(line, xE));
            }

            return result.ToArray();
        }
    }
}
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
			if (User.Identity.IsAuthenticated)
			{
				var userId = User.Identity.GetUserId();
				var userData = _uow.UserDataRepository.GetById(userId);
				if (userData != null)
				{
					var userDevices = userData.UserDevices;
					var deviceCount = userDevices.Count;

					ViewBag.DevicesCount = deviceCount;
				}
			}

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
			var deviceTemperatures = new List<object> { new IComparable[] { "Time", "Measured", "Predicted" } };

			var temperatures = userDevice.Device.Temperatures.Reverse().Take(10).Reverse().ToArray();
			List<double> arguments = new List<double>();
			List<double> temperatureValues = new List<double>();

			DateTime time;
			double temperature;

		

			//for (var i = 0; i < 10 && temperatures.Count < 10; ++i)
			//{
			//	temperatures.Add(new Temperature
			//	{
			//		Value = 0,
			//		Time = DateTime.Now
			//	});
			//}

			var count = temperatures.Count();
			var extend = new double[5];

			for (int i = 0; i < 5; i++)
			{
				extend[i] = i + 10;
			}

			for (var i = 0; i < count - 1; ++i)
			{
				temperature = temperatures[i].Value;
				time = temperatures[i].Time;
				arguments.Add(i);
				temperatureValues.Add(temperature);
				deviceTemperatures.Add(new IComparable[] {time.ToShortTimeString(), temperature, null});
			}

			while (temperatureValues.Count < 10)
			{
				temperatureValues.Insert(0, 0D);
				DateTime deviceTemperature;
				DateTime temp;
				if (temperatureValues.Count < 2)
				{
					deviceTemperature = DateTime.Now;
					temp = deviceTemperature.AddMinutes(-1);
					deviceTemperatures.Insert(1, new IComparable[] {temp.ToShortTimeString(), 0, null});
				}
				else
				{
					var temp1 = deviceTemperatures[1] as IComparable[];
					if (temp1 == null) continue;

					var tempTemperature = temp1[0] as string;
					var success = DateTime.TryParse(tempTemperature, out deviceTemperature);
					if (!success)
						deviceTemperature = DateTime.Now;
					temp = deviceTemperature.AddMinutes(-1);

					deviceTemperatures.Insert(1, new IComparable[] { temp.ToShortTimeString(), 0, null });
				}
				
				if (arguments.Count > 0)
					arguments.Add(arguments.Last() + 1);
				else
					arguments.Add(0);
			}

			if (count == 10)
			{
				temperature = temperatures[count - 1].Value;
				time = temperatures[count - 1].Time;
				arguments.Add(count - 1);
				temperatureValues.Add(temperature);
				deviceTemperatures.Add(new IComparable[] {time.ToShortTimeString(), temperature, temperature });
			}
			else
			{
				time = DateTime.Now;
				deviceTemperatures.Add(new IComparable[] { time.ToShortTimeString(), 0, 0});
			}

			var interpolantValue = Interpolant(arguments.ToArray(), temperatureValues.ToArray(), extend);
			for (var i = 0; i < interpolantValue.Length; ++i)
			{
				var time2 = time.AddMinutes(i + 1);
				deviceTemperatures.Add(new IComparable[] {time2.ToShortTimeString(), null, interpolantValue[i] });
			}

			return deviceTemperatures;
		}

		private double[] Interpolant(double[] arguments, double[] values, double[] extendsArguments)
		{
			alglib.spline1dinterpolant line;

			//This is govnocode here
			var args = arguments.ToList();
			var lastArg = extendsArguments[extendsArguments.Length - 1];
			args.Add(lastArg);
			var vals = values.ToList();

			double rsquare;
			double yintercept;
			double slope;
			LinearRegression(arguments, values, 0, arguments.Length, out rsquare, out yintercept, out slope);
			vals.Add(slope * lastArg + yintercept);

			alglib.spline1dbuildakima(args.ToArray(), vals.ToArray(), out line);

			var result = new List<double>();
			foreach (var xE in extendsArguments)
			{
				result.Add(alglib.spline1dcalc(line, xE));
			}

			return result.ToArray();
		}

		/// <summary>
		/// Fits a line to a collection of (x,y) points.
		/// </summary>
		/// <param name="xVals">The x-axis values.</param>
		/// <param name="yVals">The y-axis values.</param>
		/// <param name="inclusiveStart">The inclusive inclusiveStart index.</param>
		/// <param name="exclusiveEnd">The exclusive exclusiveEnd index.</param>
		/// <param name="rsquared">The r^2 value of the line.</param>
		/// <param name="yintercept">The y-intercept value of the line (i.e. y = ax + b, yintercept is b).</param>
		/// <param name="slope">The slop of the line (i.e. y = ax + b, slope is a).</param>
		public static void LinearRegression(double[] xVals, double[] yVals,
											int inclusiveStart, int exclusiveEnd,
											out double rsquared, out double yintercept,
											out double slope)
		{
			double sumOfX = 0;
			double sumOfY = 0;
			double sumOfXSq = 0;
			double sumOfYSq = 0;
			double ssX = 0;
			double ssY = 0;
			double sumCodeviates = 0;
			double sCo = 0;
			double count = exclusiveEnd - inclusiveStart;

			for (int ctr = inclusiveStart; ctr < exclusiveEnd; ctr++)
			{
				double x = xVals[ctr];
				double y = yVals[ctr];
				sumCodeviates += x * y;
				sumOfX += x;
				sumOfY += y;
				sumOfXSq += x * x;
				sumOfYSq += y * y;
			}
			ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
			ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
			double RNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
			double RDenom = (count * sumOfXSq - (sumOfX * sumOfX))
			 * (count * sumOfYSq - (sumOfY * sumOfY));
			sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

			double meanX = sumOfX / count;
			double meanY = sumOfY / count;
			double dblR = RNumerator / Math.Sqrt(RDenom);
			rsquared = dblR * dblR;
			yintercept = meanY - ((sCo / ssX) * meanX);
			slope = sCo / ssX;
		}
	}
}
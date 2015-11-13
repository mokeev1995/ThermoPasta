using System;
using System.Linq;
using System.Web.Mvc;
using Portal.DAL;
using Portal.Models.CodeFirstModels;

namespace Portal.Controllers
{
    public class InteractionController : Controller
    {
        private readonly IUnitOfWork _uow;

        public InteractionController(IUnitOfWork uow)
        {
            _uow = uow;
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

            return "Error.";
        }

        [HttpPost]
        public int AddTemperature(string id, int value)
        {
            var devices = _uow.DeviceRepository.GetAll().Where(d => d.Id == id).ToList();

            if (devices.Count > 0)
            {
                var newTemperature = new Temperature
                {
                    DeviceId = id,
                    Time = DateTime.Now,
                    Value = value
                };

                _uow.TemperatureRepository.Insert(newTemperature);
                _uow.Save();

                var period = devices[0].Period;

                return period;
            }

            return 1;
        }
    }
}

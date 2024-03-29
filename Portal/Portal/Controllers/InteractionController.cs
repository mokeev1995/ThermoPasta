﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Portal.DAL;
using Portal.Models.CodeFirstModels;

namespace Portal.Controllers
{
    public class InteractionController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private static readonly string codes = "codes";
        public InteractionController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public string GetCheckCode(string id)
        {
            if (id != null)
            {
                CheckAndAddDevice(id);

                if (!_uow.CheckCodeRepository.GetAll().Any(cc => cc.Id == id))
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

                return _uow.CheckCodeRepository.GetById(id).Code;
            }

            return "Error.";
        }

        private void CheckAndAddDevice(string id)
        {
            if (!_uow.DeviceRepository.GetAll().Any(d => d.Id == id))
            {
                _uow.DeviceRepository.Insert(new Device
                {
                    Id = id
                });
            }
        }

        [HttpGet]
        public string AddTemperature(string id, int ambientTemp, int objectTemp)
        {
            CheckAndAddDevice(id);

            var newTemperature = new Temperature
            {
                DeviceId = id,
                Time = DateTime.Now,
                Value = ambientTemp
            };

            _uow.TemperatureRepository.Insert(newTemperature);
            _uow.Save();


            return "OK";
        }
    }
}


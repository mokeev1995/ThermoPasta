﻿using System;
using Portal.Models.CodeFirstModels;
using Portal.Models.Context;

namespace Portal.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private readonly PortalDbContext _context = new PortalDbContext();
        private IRepository<AspNetUser> _aspNetUserRepository;
        private IRepository<UserData> _userDataRepository;
        private IRepository<Device> _deviceRepository;
        private IRepository<Profile> _profileRepository;
        private IRepository<Interval> _intervalRepository;
        private IRepository<CheckCode> _checkCodeRepository;
        private IRepository<Temperature> _temperatureRepository;
        private IRepository<UserDevice> _userDeviceRepository;

        public IRepository<AspNetUser> AspNetUserRepository
        {
            get
            {
                if (_aspNetUserRepository == null)
                {
                    _aspNetUserRepository = new GenericRepository<AspNetUser>(_context);
                }
                return _aspNetUserRepository;
            }
        }

        public IRepository<UserData> UserDataRepository
        {
            get
            {
                if (_userDataRepository == null)
                {
                    _userDataRepository = new GenericRepository<UserData>(_context);
                }
                return _userDataRepository;
            }
        }

        public IRepository<Device> DeviceRepository
        {
            get
            {
                if (_deviceRepository == null)
                {
                    _deviceRepository = new GenericRepository<Device>(_context);
                }
                return _deviceRepository;
            }
        }

        public IRepository<Profile> ProfileRepository
        {
            get
            {
                if (_profileRepository == null)
                {
                    _profileRepository = new GenericRepository<Profile>(_context);
                }
                return _profileRepository;
            }
        }

        public IRepository<Interval> IntervalRepository
        {
            get
            {
                if (_intervalRepository == null)
                {
                    _intervalRepository = new GenericRepository<Interval>(_context);
                }
                return _intervalRepository;
            }
        }

        public IRepository<CheckCode> CheckCodeRepository
        {
            get
            {
                if (_checkCodeRepository == null)
                {
                    _checkCodeRepository = new GenericRepository<CheckCode>(_context);
                }
                return _checkCodeRepository;
            }
        }


        public IRepository<UserDevice> UserDeviceRepository
        {
            get
            {
                if (_userDeviceRepository == null)
                {
                    _userDeviceRepository = new GenericRepository<UserDevice>(_context);
                }
                return _userDeviceRepository;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }


        public IRepository<Temperature> TemperatureRepository
        {
            get
            {
                if (_temperatureRepository == null)
                {
                    _temperatureRepository = new GenericRepository<Temperature>(_context);
                }
                return _temperatureRepository;
            }
        }
    }
}
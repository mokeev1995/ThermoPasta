using System;
using Portal.Models.CodeFirstModels;

namespace Portal.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<AspNetUser> AspNetUserRepository { get; }
        IRepository<UserData> UserDataRepository { get; }
        IRepository<Device> DeviceRepository { get; }
        IRepository<Profile> ProfileRepository { get; }
        IRepository<Interval> IntervalRepository { get; }
        IRepository<CheckCode> CheckCodeRepository { get; }
        IRepository<Temperature> TemperatureRepository { get; }

        void Save();
    }
}
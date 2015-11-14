using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Portal.Models.CodeFirstModels;
using Portal.Models.Context;

namespace Portal.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<PortalDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(PortalDbContext context)
        {
            var User = new AspNetRole
            {
                Id = Role.User.ToString(),
                Name = Role.User.ToString()
            };
            var Admin = new AspNetRole
            {
                Id = Role.Admin.ToString(),
                Name = Role.Admin.ToString()
            };
            context.AspNetRoles.AddOrUpdate(r => r.Id, User, Admin);
            context.SaveChanges();

            var Alex = new AspNetUser
            {
                Id = "1a40e3cb-71fc-4e6f-8b43-e6081ab390fe",
                Email = "Alex@yar.ru",
                PasswordHash = "AMswa3Q0GDO2SMS2R7CB4/L0ZUHGzCd6rPDbs3NxHTp7xVNuACBhSpkMMPjV3zhtEA==",
                SecurityStamp = "dce7b5c9-9841-4571-996c-4b8f488e6510",
                UserName = "Alex@yar.ru"
            };
            Alex.AspNetRoles.Add(User);
            var AlexData = new UserData
            {
                Id = Alex.Id,
                FirstName = "Alex",
                LastName = "Grim",
            };
            context.UserDatas.AddOrUpdate(e => e.FirstName, AlexData);
            context.SaveChanges();

            var Kirill = new AspNetUser
            {
                Id = "3f2c9533-ed2e-4a2e-9c68-27f431c2cc0c",
                Email = "Kirill@yar.ru",
                PasswordHash = "ACarxdDj/3gCH/erg4xKsUZdoi4mVwmqmSXOOZy/BYqhS2GCndmAKRiqkGLhiOWvZA==",
                SecurityStamp = "1def27e4-2d64-45e3-b311-b3e17b304766",
                UserName = "Kirill@yar.ru"
            };
            Kirill.AspNetRoles.Add(Admin);
            var KirillData = new UserData
            {
                Id = Kirill.Id,
                FirstName = "Kirill",
                LastName = "Vinogradov",
            };
            context.UserDatas.AddOrUpdate(e => e.FirstName, KirillData);
            context.SaveChanges();

            context.AspNetUsers.AddOrUpdate(Alex, Kirill);
            context.SaveChanges();

            var teapotProfile = new Profile
            {
                Title = "teapot#1",
                Description = "some teapot description"
            };
            var poolProfile = new Profile
            {
                Title = "pool#1",
                Description = "some pool description"
            };
            var bathProfile = new Profile
            {
                Title = "bath#1",
                Description = "some pool description",
                UserDataId = KirillData.Id
            };
            context.Profiles.AddOrUpdate(p => p.Title, teapotProfile, poolProfile, bathProfile);
            context.SaveChanges();

            var teapotIntervals = new[]
            {
                new Interval
                {
                    ProfileId = 1,
                    Description = "cold",
                    Start = 1,
                    End = 25
                },
                new Interval
                {
                    ProfileId = 1,
                    Description = "warm",
                    Start = 26,
                    End = 75
                },
                new Interval
                {
                    ProfileId = 1,
                    Description = "hot",
                    Start = 76,
                    End = 100
                }
                };

            var poolIntervals = new[]
            {
                new Interval
                {
                    ProfileId = 2,
                    Description = "cool",
                    Start = 1,
                    End = 25
                },
                new Interval
                {
                    ProfileId = 2,
                    Description = "let's swim!",
                    Start = 26,
                    End = 30
                }
                };

            var bathIntervals = new[]
            {
                new Interval
                {
                    ProfileId = 3,
                    Description = "OK;)",
                    Start = -100,
                    End = 100
                }
            };

            context.Intervals.AddOrUpdate(i => i.Description, teapotIntervals);
            context.Intervals.AddOrUpdate(i => i.Description, poolIntervals);
            context.Intervals.AddOrUpdate(i => i.Description, bathIntervals);
            context.SaveChanges();

            var teapot = new Device
            {
                Id = "teapotMAC",

            };
            var pool = new Device
            {
                Id = "poolMAC",

            };
            context.Devices.AddOrUpdate(d => d.Id, teapot, pool);
            context.SaveChanges();

            var teapotUserDevice = new UserDevice
            {
                UserDataId = KirillData.Id,
                DeviceId = teapot.Id,
                ProfileId = 1,
                Title = "electric teapot in kitchen",
                Period = 1
            };
            var poolUserDevice = new UserDevice
            {
                DeviceId = pool.Id,
                ProfileId = 2,
                Title = "pool in front of house",
                UserDataId = KirillData.Id,
                Period = 30
            };
            context.UserDevices.AddOrUpdate(d => d.Title, teapotUserDevice, poolUserDevice);
            context.SaveChanges();

            context.Temperatures.AddOrUpdate(t=>t.Time,
                new Temperature
                {
                    DeviceId = teapot.Id,
                    Value = 53,
                    Time = DateTime.Now.AddMinutes(-2)
                },
                new Temperature
                {
                    DeviceId = pool.Id,
                    Value = 27,
                    Time = DateTime.Now.AddMinutes(-1) 
                });
            context.SaveChanges();
            //var newCode = new CheckCode
            //{
            //    Id = "mac123456",
            //    Code = "1324",
            //    Time = DateTime.Now.AddMinutes(-5)
            //};

            //context.CheckCodes.AddOrUpdate(d => d.Id, newCode);
            //context.SaveChanges();
        }
    }
}
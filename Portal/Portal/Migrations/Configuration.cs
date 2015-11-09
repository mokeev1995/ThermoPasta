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
        }
    }
}
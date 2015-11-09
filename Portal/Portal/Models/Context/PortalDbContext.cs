using System.Data.Entity;
using Portal.Migrations;
using Portal.Models.CodeFirstModels;

namespace Portal.Models.Context
{
    public class PortalDbContext : DbContext
    {
        public PortalDbContext()
            : base("name=DefaultConnection")
        {
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<PortalDbContext, Configuration>("DefaultConnection"));
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<UserData> UserDatas { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Interval> Intervals { get; set; }
      
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
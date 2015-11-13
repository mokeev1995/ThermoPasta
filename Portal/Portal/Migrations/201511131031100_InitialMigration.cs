namespace Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Temperatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceId = c.String(nullable: false, maxLength: 128),
                        Value = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Devices", t => t.DeviceId, cascadeDelete: true)
                .Index(t => t.DeviceId);
            
            DropColumn("dbo.Devices", "CurrentTemparature");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Devices", "CurrentTemparature", c => c.Int(nullable: false));
            DropForeignKey("dbo.Temperatures", "DeviceId", "dbo.Devices");
            DropIndex("dbo.Temperatures", new[] { "DeviceId" });
            DropTable("dbo.Temperatures");
        }
    }
}

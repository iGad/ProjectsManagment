namespace ProjectsManagment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onemore : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventsUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        Seen = c.Boolean(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.Events", "EventsUsers_Id", c => c.Int());
            AddColumn("dbo.AspNetUsers", "EventsUsers_Id", c => c.Int());
            CreateIndex("dbo.Events", "EventsUsers_Id");
            CreateIndex("dbo.AspNetUsers", "EventsUsers_Id");
            AddForeignKey("dbo.Events", "EventsUsers_Id", "dbo.EventsUsers", "Id");
            AddForeignKey("dbo.AspNetUsers", "EventsUsers_Id", "dbo.EventsUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "EventsUsers_Id", "dbo.EventsUsers");
            DropForeignKey("dbo.EventsUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "EventsUsers_Id", "dbo.EventsUsers");
            DropForeignKey("dbo.EventsUsers", "EventId", "dbo.Events");
            DropIndex("dbo.EventsUsers", new[] { "User_Id" });
            DropIndex("dbo.EventsUsers", new[] { "EventId" });
            DropIndex("dbo.AspNetUsers", new[] { "EventsUsers_Id" });
            DropIndex("dbo.Events", new[] { "EventsUsers_Id" });
            DropColumn("dbo.AspNetUsers", "EventsUsers_Id");
            DropColumn("dbo.Events", "EventsUsers_Id");
            DropTable("dbo.EventsUsers");
        }
    }
}

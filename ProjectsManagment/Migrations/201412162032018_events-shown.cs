namespace ProjectsManagment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eventsshown : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventsUsers", "Shown", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventsUsers", "Shown");
        }
    }
}

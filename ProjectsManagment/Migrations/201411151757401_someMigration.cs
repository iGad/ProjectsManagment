namespace ProjectsManagment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class someMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Partitions", "Creator_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.Partitions", new[] { "Creator_Id" });
            AlterColumn("dbo.Partitions", "Creator_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Partitions", "Creator_Id");
            AddForeignKey("dbo.Partitions", "Creator_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Partitions", "Creator_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.Partitions", new[] { "Creator_Id" });
            AlterColumn("dbo.Partitions", "Creator_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Partitions", "Creator_Id");
            AddForeignKey("dbo.Partitions", "Creator_Id", "dbo.ApplicationUsers", "Id");
        }
    }
}

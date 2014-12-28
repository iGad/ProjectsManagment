namespace ProjectsManagment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ProjectController : DbMigration
    {
        public override void Up()
        {
            //в notepad++ есть новый вариант. откатить этот вариант и заменить содержимое файла
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            RenameTable(name: "dbo.AspNetRoles", newName: "IdentityRoles");
            RenameTable(name: "dbo.AspNetUserRoles", newName: "IdentityUserRoles");
            RenameTable(name: "dbo.AspNetUsers", newName: "ApplicationUsers");
            RenameTable(name: "dbo.AspNetUserClaims", newName: "IdentityUserClaims");
            RenameTable(name: "dbo.AspNetUserLogins", newName: "IdentityUserLogins");
            DropForeignKey("dbo.Projects", "Creator_Id", "dbo.AspNetUsers");
            DropIndex("dbo.IdentityRoles", "RoleNameIndex");
            DropIndex("dbo.IdentityRoles", "UserNameIndex");
            DropIndex("dbo.IdentityUserRoles", new[] { "RoleId" });
            DropIndex("dbo.IdentityUserRoles", new[] { "UserId" });
            DropIndex("dbo.ApplicationUsers", "UserNameIndex");
            DropIndex("dbo.IdentityUserClaims", new[] { "UserId" });
            DropIndex("dbo.IdentityUserLogins", new[] { "UserId" });
            AddColumn("dbo.IdentityUserRoles", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.IdentityUserRoles", "IdentityRole_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.IdentityUserClaims", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.IdentityUserLogins", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.IdentityRoles", "Name", c => c.String());
            //AlterColumn("dbo.ApplicationUsers", "Email", c => c.String());
            //AlterColumn("dbo.ApplicationUsers", "UserName", c => c.String());
            //AlterColumn("dbo.IdentityUserClaims", "UserId", c => c.String());
            CreateIndex("dbo.IdentityUserClaims", "ApplicationUser_Id");
            CreateIndex("dbo.IdentityUserLogins", "ApplicationUser_Id");
            CreateIndex("dbo.IdentityUserRoles", "ApplicationUser_Id");
            CreateIndex("dbo.IdentityUserRoles", "IdentityRole_Id");
            CreateIndex("dbo.IdentityUserRoles", "UserId");
            CreateIndex("dbo.IdentityUserRoles", "RoleId");
            //CreateIndex("dbo.ApplicationUsers", "UserName", unique: true, name: "UserNameIndex");
            //CreateIndex("dbo.IdentityRoles", "Name", unique: true, name: "RoleNameIndex");
            AddForeignKey("dbo.IdentityUserRoles", "RoleId", "dbo.IdentityRoles", "Id");
            AddForeignKey("dbo.IdentityUserClaims", "UserId", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.IdentityUserLogins", "UserId", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.IdentityUserRoles", "UserId", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.Projects", "Creator_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: false);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Projects", "Creator_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.IdentityUserRoles", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.IdentityUserLogins", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.IdentityUserClaims", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.IdentityUserRoles", "IdentityRole_Id", "dbo.IdentityRoles");
            DropIndex("dbo.IdentityUserRoles", new[] { "IdentityRole_Id" });
            DropIndex("dbo.IdentityUserRoles", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserRoles", new[] { "RoleId" });
            DropIndex("dbo.IdentityUserRoles", new[] { "UserId" });
            DropIndex("dbo.IdentityUserLogins", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserClaims", new[] { "ApplicationUser_Id" });
            //AlterColumn("dbo.IdentityUserClaims", "UserId", c => c.String(nullable: false, maxLength: 128));
            //AlterColumn("dbo.ApplicationUsers", "UserName", c => c.String(nullable: false, maxLength: 256));
            //AlterColumn("dbo.ApplicationUsers", "Email", c => c.String(maxLength: 256));
            AlterColumn("dbo.IdentityRoles", "Name", c => c.String(nullable: false, maxLength: 256));
            DropColumn("dbo.IdentityUserLogins", "ApplicationUser_Id");
            DropColumn("dbo.IdentityUserClaims", "ApplicationUser_Id");
            DropColumn("dbo.IdentityUserRoles", "IdentityRole_Id");
            DropColumn("dbo.IdentityUserRoles", "ApplicationUser_Id");
            CreateIndex("dbo.IdentityUserLogins", "UserId");
            CreateIndex("dbo.IdentityUserClaims", "UserId");
            //CreateIndex("dbo.ApplicationUsers", "UserName", unique: true, name: "UserNameIndex");
            CreateIndex("dbo.IdentityUserRoles", "UserId");
            CreateIndex("dbo.IdentityUserRoles", "RoleId");
            //CreateIndex("dbo.IdentityRoles", "Name", unique: true, name: "RoleNameIndex");
            AddForeignKey("dbo.Projects", "Creator_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles", "Id", cascadeDelete: true);
            RenameTable(name: "dbo.IdentityUserLogins", newName: "AspNetUserLogins");
            RenameTable(name: "dbo.IdentityUserClaims", newName: "AspNetUserClaims");
            RenameTable(name: "dbo.ApplicationUsers", newName: "AspNetUsers");
            RenameTable(name: "dbo.IdentityUserRoles", newName: "AspNetUserRoles");
            RenameTable(name: "dbo.IdentityRoles", newName: "AspNetRoles");
        }
    }
}

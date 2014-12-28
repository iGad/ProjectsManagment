namespace ProjectsManagment.Migrations
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using ProjectsManagment.Models;
    using System.Security.Claims;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web;
    using System.Collections.Generic;
    using System.Collections;

    internal sealed class Configuration : DbMigrationsConfiguration<ProjectsManagment.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "ProjectsManagment.Models.ApplicationDbContext";
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(ProjectsManagment.Models.ApplicationDbContext context)
        {
            List<string> rolesName = new List<string>() { "Руководитель направления", "Главный инженер проекта", "Исполнитель", "Администратор", "Директор" };
            for (int i = 0; i < rolesName.Count; i++)
            {
                if (context.Roles.Where(k=>k.Name==rolesName[i]).Select(r => r).Take(1) == null)
                    context.Roles.Add(new IdentityRole(rolesName[i]));
                
            }
            context.SaveChanges();
            //var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            //const string name = "admin@admin.ru";
            //const string password = "15935712gfdtk";
            //const string roleName = "Директор";

            ////Create Role Admin if it does not exist
            //var role = roleManager.FindByName(roleName);
            //if (role == null)
            //{
            //    role = new IdentityRole(roleName);
            //    var roleresult = roleManager.Create(role);
            //}

            //var user = userManager.FindByName(name);
            //if (user == null)
            //{
            //    user = new ApplicationUser { UserName = name, Email = name, Name = "Павел", Surname = "Одегов", Birthday = DateTime.Parse("1993-12-07") };
            //    var result = userManager.Create(user, password);
            //    result = userManager.SetLockoutEnabled(user.Id, false);
            //}

            //// Add user admin to Role Admin if not already added
            //var rolesForUser = userManager.GetRoles(user.Id);
            //if (!rolesForUser.Contains(role.Name))
            //{
            //    var result = userManager.AddToRole(user.Id, role.Name);
            //}
            //context.Roles.AddOrUpdate(r => r,
            //    new IdentityRole() { Name = "Директор", },
            //    new IdentityRole("Главный инженер проекта"),
            //    new IdentityRole("Руководитель направления"),
            //    new IdentityRole("Исполнитель"));
            //context.Users.Add(new ApplicationUser()
            //{
            //    UserName = "a@a.ru",
            //    Email = "a@a.ru",
            //    Birthday = DateTime.Parse("1993-12-07"),
            //    Name = "Павел",
            //     EmailConfirmed = true                  
            //});
            
            //ApplicationDbInitializer.InitializeIdentityForEF(context);
            //  This method will be called after migrating to the latest version.
            
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectsManagment.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Необходимо ввести имя", AllowEmptyStrings=false)]
        [Display(Name="Имя")]
        //[StringLength(32,MinimumLength=2,ErrorMessage = "Длина имени должна быть от 2х до 32х букв")]
        public string Name { get; set; }
        [Display(Name = "Фамилия")]
        ///[StringLength(32, MinimumLength = 2, ErrorMessage = "Длина фамилии должна быть от 2х до 32х букв")]
        public string Surname { get; set; }
        [Display(Name = "Отчество")]
        //[StringLength(32, MinimumLength = 2, ErrorMessage = "Длина отчества должна быть от 2х до 32х букв")]
        public string Fathername { get; set; }
        [Required(ErrorMessage="Необходимо ввести дату рождения")]
        [DataType(DataType.Date,ErrorMessage = "Введенное значение должно быть датой в формате дд.мм.гггг")]
        public DateTime Birthday { get; set; }
        
        public virtual ICollection<Project> ProjectCreators { get; set; }
        public virtual ICollection<Project> ProjectsManagers { get; set; }
        
        public virtual ICollection<Stage> StagesCreators { get; set; }
        public virtual ICollection<Stage> StagesManagers { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Event> EventMakers { get; set; }
        public virtual ICollection<Event> EventCreators { get; set; }
        public virtual ICollection<Task> TaskCreators { get; set; }
        public virtual ICollection<Task> TaskExecutors { get; set; }
        public virtual ICollection<Partition> PartitionCreators { get; set; }
        public virtual ICollection<Partition> PartitionManagers { get; set; }
        
        public ApplicationUser()
        {
            ProjectCreators = new HashSet<Project>();
            ProjectsManagers = new HashSet<Project>();
            Comments = new HashSet<Comment>();
            EventMakers = new HashSet<Event>();
            EventCreators = new HashSet<Event>();
            TaskCreators = new HashSet<Task>();
            TaskExecutors = new HashSet<Task>();
            PartitionCreators = new HashSet<Partition>();
            PartitionManagers = new HashSet<Partition>();
            StagesCreators = new HashSet<Stage>();
            StagesManagers = new HashSet<Stage>();
        }
        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)//"DefaultConnection"
        {
            //Database.SetInitializer<ApplicationDbContext>(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
            //Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Project>().
            //    HasRequired(u => u.Creator).
            //    WithOptional().
            //    WillCascadeOnDelete(false);
            //modelBuilder.Entity<Project>().
            //    HasOptional(u => u.Manager);

            //modelBuilder.Entity<IdentityUserLogin>().HasKey(k => new { k.UserId, k.ProviderKey, k.LoginProvider });
            //modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            //modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            //Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<ProjectsManagment.Models.Project> Projects { get; set; }

        public System.Data.Entity.DbSet<ProjectsManagment.Models.TasksState> TasksStates { get; set; }

        public System.Data.Entity.DbSet<ProjectsManagment.Models.Stage> Stages { get; set; }

        public System.Data.Entity.DbSet<ProjectsManagment.Models.Partition> Partitions { get; set; }
        public System.Data.Entity.DbSet<ProjectsManagment.Models.TablesType> TableTypes { get; set; }
        public System.Data.Entity.DbSet<ProjectsManagment.Models.Event> Events { get; set; }
        public System.Data.Entity.DbSet<ProjectsManagment.Models.EventsUsers> EventsUsers { get; set; }
        public System.Data.Entity.DbSet<ProjectsManagment.Models.EventsType> EventsTypes { get; set; }

        public System.Data.Entity.DbSet<ProjectsManagment.Models.Comment> Comments { get; set; }

        public System.Data.Entity.DbSet<ProjectsManagment.Models.ProjectsFile> ProjectsFiles { get; set; }

        public System.Data.Entity.DbSet<ProjectsManagment.Models.Task> Tasks { get; set; }

        public System.Data.Entity.DbSet<ProjectsManagment.Models.Dependency> Dependencies { get; set; }

        public System.Data.Entity.DbSet<ProjectsManagment.Models.ProjectsFolder> ProjectsFolders { get; set; }
    }
}
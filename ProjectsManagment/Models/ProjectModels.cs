using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ProjectsManagment.Models
{
    
    public class TasksState
    {
        public TasksState()
        {
            this.Projects = new HashSet<Project>();
            this.Stages = new HashSet<Stage>();
            this.Partitions = new HashSet<Partition>();
            this.Tasks = new HashSet<Task>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Partition> Partitions { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Stage> Stages { get; set; }
    }
    
    public class TablesType
    {
        public TablesType()
        {
            this.Comments = new HashSet<Comment>();
            this.Events = new HashSet<Event>();
            this.Dependencies = new HashSet<Dependency>();
        }
        public int Id { get; set; }
        [Display(Name="Название таблицы")]
        [Required(ErrorMessage="Необходимо ввести название таблицы")]
        public string TableName { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Dependency> Dependencies { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
    
    public class Comment
    {
        public Comment()
        {
            this.Events = new HashSet<Event>();
        }
        public int Id { get; set; }
        public int TableId { get; set; }
        //public string UserCreator { get; set; }
        public int ItemId { get; set; }
        [Required(ErrorMessage="Необходимо ввести текст комментария")]
        [Display(Name="Текст:")]
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public bool Bad { get; set; }
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("TableId")]
        public virtual TablesType Table { get; set; }
        public virtual Project Project { get; set; }
        public virtual Stage Stage { get; set; }
        public virtual Partition Partition { get; set; }
        public virtual Task Task { get; set; }
        public virtual ProjectsFile File { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }


    public enum DependencyType
    {
        Task,
        File
    }

    public enum DependencyTable
    {
        Task,
        Partition,
        Stage,
        Project,
        File,
        Comment,
        User
    }

    public class Dependency
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public int ItemId { get; set; }
        [ForeignKey("TableId")]
        public virtual TablesType Table { get; set; }

        public virtual Project Project { get; set; }
        public virtual Stage Stage { get; set; }
        public virtual Partition Partition { get; set; }
        public virtual Task Task { get; set; }

        public virtual Task TaskDep { get; set; }
        //public virtual Project ProjectDep { get; set; }
        //public virtual Stage StageDep { get; set; }
        //public virtual Partition PartitionDep { get; set; }
        public virtual ProjectsFile FileDep { get; set; }
    }

    public class Task
    {
        public Task()
        {
            this.Comments = new HashSet<Comment>();
            //this.Folders = new HashSet<ProjectsFolder>();
            //this.Files = new HashSet<ProjectsFile>();
            this.Events = new HashSet<Event>();
            this.Dependencies = new HashSet<Dependency>();
        }
        public int Id { get; set; }
        //public string UserCreator { get; set; }
        //public string UserExecutor { get; set; }
         
        public int PartitionId { get; set; }
        [Required]
        public bool IsPreFinish { get; set; }
        [Required]
        public bool IsFinish { get; set; }
         [Display(Name = "Дата создания")]
        public DateTime CreationDate { get; set; }
         [Display(Name = "Дедлайн")]
        public DateTime DeadLine { get; set; }
        public int StateId { get; set; }
        [Required]
        [Display(Name = "Название задания")]
        public string Name { get; set; }
         [Display(Name = "Описание")]
        public string Description { get; set; }
        [ForeignKey("StateId")]
        [Display(Name = "Состояние")]
        public virtual TasksState State { get; set; }
        [Required]
        [Display(Name = "Создатель")]
        public virtual ApplicationUser Creator { get; set; }
         [Display(Name = "Исполнитель")]
        public virtual ApplicationUser Executor { get; set; }
        [ForeignKey("PartitionId")]
        [Display(Name = "Раздел")]
        public virtual Partition Partition { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        //public virtual ICollection<ProjectsFolder> Folders { get; set; }
        //public virtual ICollection<ProjectsFile> Files { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Dependency> Dependencies { get; set; }
    }
    
    public class Partition
    {
        public Partition()
        {
            this.Comments = new HashSet<Comment>();
            //this.Folders = new HashSet<ProjectsFolder>();
            //this.Files = new HashSet<ProjectsFile>();
            this.Events = new HashSet<Event>();
            this.Tasks = new HashSet<Task>();
            this.Dependencies = new HashSet<Dependency>();
        }
        public int Id { get; set; }
        //public string UserCreator { get; set; }
        //public string UserManager { get; set; }
        public int StageId { get; set; }
        [Required]
        [Display(Name = "Дата создания")]
        public DateTime CreationDate { get; set; }
         [Display(Name = "Дедлайн")]
        public DateTime DeadLine { get; set; }
        public int StateId { get; set; }
        [Required]
        [Display(Name = "Название раздела")]
        public string Name { get; set; }
         [Display(Name = "Описание")]
        public string Description { get; set; }
        [Required]
        public bool IsPreFinish { get; set; }
        [Required]
        public bool IsFinish { get; set; }
        [ForeignKey("StateId")]
        [Display(Name = "Состояние")]
        public virtual TasksState State { get; set; }
        [Required]
        [Display(Name = "Создатель")]
        public virtual ApplicationUser Creator { get; set; }
        [ForeignKey("StageId")]
        [Display(Name = "Стадия")]
        public virtual Stage Stage { get; set; }
        [Display(Name = "Менеджер")]
        public virtual ApplicationUser Manager { get; set; }
        [Display(Name="Задания")]
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Dependency> Dependencies { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        //public virtual ICollection<ProjectsFolder> Folders { get; set; }
        //public virtual ICollection<ProjectsFile> Files { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
    
    public class Stage
    {
        public Stage()
        {
            this.Comments = new HashSet<Comment>();
            //this.Folders = new HashSet<ProjectsFolder>();
            //this.Files = new HashSet<ProjectsFile>();
            this.Events = new HashSet<Event>();
            this.Partitions = new HashSet<Partition>();
            this.Dependencies = new HashSet<Dependency>();
        }
        public int Id { get; set; }
        //public string UserCreator { get; set; }
        //public string UserManager { get; set; }
        public int ProjectId { get; set; }
        [Display(Name="Дата создания")]
        public DateTime CreationDate { get; set; }
        public int StateId { get; set; }
        [Required(AllowEmptyStrings=false)]
        [Display(Name = "Название стадии")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Отправлено на проверку")]
        public bool IsPreFinish { get; set; }
        [Required]
        [Display(Name = "Закончен")]
        public bool IsFinish { get; set; }
        [ForeignKey("StateId")]
        [Display(Name = "Состояние")]
        public virtual TasksState State { get; set; }
        [Display(Name = "Создатель")]
        public virtual ApplicationUser Creator { get; set; }
        [Display(Name = "Менеджер")]
        public virtual ApplicationUser Executor { get; set; }
        [ForeignKey("ProjectId")]
        [Display(Name = "Проект")]
        public virtual Project Project { get; set; }
        [Display(Name="Разделы")]
        public virtual ICollection<Partition> Partitions { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        //public virtual ICollection<ProjectsFolder> Folders { get; set; }
        //public virtual ICollection<ProjectsFile> Files { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Dependency> Dependencies { get; set; }
    }
    public class Project
    {
        public Project()
        {
            this.Comments = new HashSet<Comment>();
            this.Folders = new HashSet<ProjectsFolder>();
            this.Files = new HashSet<ProjectsFile>();
            this.Events = new HashSet<Event>();
            this.Stages = new HashSet<Stage>();
        }   
        public int Id { get; set; }
        //public string UserCreator { get; set; }
        //public string UserManager { get; set; }
        [Required]
        [Display(Name="Дата создания")]
        public DateTime CreationDate { get; set; }
        [Display(Name = "Дедлайн")]
        public virtual DateTime DeadLine { get; set; }
        public int StateId { get; set; }
        [Required]
        [Display(Name = "Название проекта")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [Display(Name="Отправлен на проверку")]
        public bool IsPreFinish { get; set; }
        [Required]
        [Display(Name="Закончен")]
        public bool IsFinish { get; set; }
        [Display(Name="Стадии")]
        public virtual ICollection<Stage> Stages { get; set; }
        [ForeignKey("StateId")]
        [Display(Name = "Состояние")]
        public virtual TasksState State { get; set; }
        [Required]
        [Display(Name = "Создатель проекта")]
        public virtual ApplicationUser Creator { get; set; }
        [Display(Name = "Главный инженер проекта")]
        public virtual ApplicationUser Manager { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<ProjectsFolder> Folders { get; set; }
        public virtual ICollection<ProjectsFile> Files { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }

    
    public class ProjectsFolder
    {
        public ProjectsFolder()
        {
            this.Files = new HashSet<ProjectsFile>();
            this.Events = new HashSet<Event>();
        }
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ParentFolderId { get; set; }
        [Required]
        public string Name { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        [ForeignKey("ParentFolderId")]
        public virtual ProjectsFolder ParentFolder { get; set; }
        public virtual ICollection<ProjectsFile> Files { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }

    public class ProjectsFile
    {
        public ProjectsFile()
        {
            this.Events = new HashSet<Event>();
            this.Comments = new HashSet<Comment>();
        }
        public int Id { get; set; }
        public int FolderId { get; set; }
        public int ProjectId { get; set; }
        public string FullPath { get; set; }
        [Required]
        public string Name { get; set; }
        [ForeignKey("FolderId")]
        public virtual ProjectsFolder Folder { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }

    public class EventsType
    {
        public EventsType()
        {
            Events = new HashSet<Event>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }

    public class Event
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        //public string UserCreator { get; set; }
        public int TableId { get; set; }    
        [Display(Name="Элемент, с которым произошло событие")]
        public int ItemId { get; set; }
        [Display(Name="Дата")]
        public DateTime Date { get; set; }
        [ForeignKey("EventId")]
        [Display(Name="Событие")]
        public virtual EventsType EventType { get; set; }
        [Display(Name="Пользователь")]
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("TableId")]
        public virtual TablesType Table { get; set; }
        
        public virtual Project EventProject { get; set; }
        public virtual Stage EventStage { get; set; }
        public virtual Partition EventPartition { get; set; }
        public virtual Task EventTask { get; set; }
        
        public virtual ApplicationUser EventUser { get; set; }
       
        public virtual ProjectsFolder EventFolder { get; set; }
        
        public virtual ProjectsFile EventFile { get; set; }
        
        public virtual Comment EventComment { get; set; }
    }

    public class EventsUsers
    {
        public int Id { get; set; }
        [Required]
        public virtual ApplicationUser User { get; set; }
        [Required]        
        public int EventId { get; set; }
        [Required]
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }
        public bool Seen { get; set; }
        public bool Shown { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
    
}
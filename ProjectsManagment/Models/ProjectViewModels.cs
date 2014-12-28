using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectsManagment.Models
{
    public class DependencyView
    {
        public int Id { get; set; }
        [Display(Name="Раздел")]
        public int PartitionId { get; set; }
        [Display(Name = "Стадия")]
        public int StageId { get; set; }
        [Display(Name = "Задание")]
        public int TaskId { get; set; }
        [Required]
        public int ItemId { get; set; }
    }
    public class ProjectDetailView
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Необходимо указать дату дедлайна")]
        [Display(Name = "Дедлайн")]
        public DateTime DeadLine { get; set; }
        [Required(ErrorMessage = "Необходимо ввести название")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Менеджер")]
        public string UserId { get; set; }

        public int ParentId { get; set; }

        public string CreatorId { get; set; }
        [Display(Name = "Завершено")]
        public bool IsFinish { get; set; }
        [Display(Name = "Завершено")]
        public bool IsPreFinish { get; set; }
        [Display(Name = "Комментарий")]
        //[Required(ErrorMessage = "Необходимо прокомментировать изменения")]
        public string Comment { get; set; }
    }

    public class ProjectView
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Необходимо указать дату дедлайна")]
        [Display(Name = "Дедлайн")]
        public DateTime DeadLine { get; set; }
        [Required(ErrorMessage = "Необходимо ввести название")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Главный инженер")]
        [Required(ErrorMessage="Необходимо выбрать главного инженера проекта")]
        public string UserId { get; set; }

        public string CreatorId { get; set; }
        [Display(Name = "Завершено")]
        public bool IsFinish { get; set; }
        [Display(Name="Комментарий")]
        public string Comment { get; set; }
    }
    public class StageView
    {
        public int Id { get; set; }        
        [Required(ErrorMessage = "Необходимо ввести название", AllowEmptyStrings=false)]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Менеджер")]
        public string UserId { get; set; }
        [Required]
        public int ParentId { get; set; }

        public string CreatorId { get; set; }
        [Display(Name = "Завершено")]
        public bool IsFinish { get; set; }
        [Display(Name = "Завершено")]
        public bool IsPreFinish { get; set; }
        [Display(Name = "Комментарий")]
        //[Required(ErrorMessage = "Необходимо прокомментировать изменения")]
        public string Comment { get; set; }
    }

    public class PartitionCreateView
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Необходимо указать дату дедлайна")]
        [Display(Name="Дедлайн")]        
        public DateTime DeadLine { get; set; }
        [Required(ErrorMessage = "Необходимо ввести название")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Менеджер")]
        public string UserId { get; set; }
        
        public int ParentId { get; set; }

        public string CreatorId { get; set; }
        [Display(Name = "Завершено")]
        public bool IsFinish { get; set; }
        [Display(Name = "Завершено")]
        public bool IsPreFinish { get; set; }
        [Display(Name = "Комментарий")]
        //[Required(ErrorMessage="Необходимо прокомментировать изменения")]
        public string Comment { get; set; }
    }
    public class TaskView
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Необходимо указать дату дедлайна")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дедлайн")]
        public DateTime DeadLine { get; set; }
        [Required(ErrorMessage = "Необходимо ввести название")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Исполнитель")]
        public string UserId { get; set; }

        public int ParentId { get; set; }

        public string CreatorId { get; set; }
        [Display(Name = "Завершено")]
        public bool IsFinish { get; set; }
        [Display(Name = "Завершено")]
        public bool IsPreFinish { get; set; }
        [Display(Name = "Комментарий")]
        //[Required(ErrorMessage="Необходимо прокомментировать изменения")]
        public string Comment { get; set; }
    }
}
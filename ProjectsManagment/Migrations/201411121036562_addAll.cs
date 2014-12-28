namespace ProjectsManagment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAll : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TableId = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        Text = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                
                .ForeignKey("dbo.TablesTypes", t => t.TableId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.TableId)
                .Index(t => t.ItemId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        TableId = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Partition_Id = c.Int(),
                        Task_Id = c.Int(),
                        Stage_Id = c.Int(),
                        EventUser_Id = c.String(maxLength: 128),
                        User_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
               
                .ForeignKey("dbo.Partitions", t => t.Partition_Id)
                .ForeignKey("dbo.Tasks", t => t.Task_Id)
                .ForeignKey("dbo.TablesTypes", t => t.TableId, cascadeDelete: false)
                .ForeignKey("dbo.Stages", t => t.Stage_Id)
                .ForeignKey("dbo.EventsTypes", t => t.EventId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.EventUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .Index(t => t.EventId)
                .Index(t => t.TableId)
                .Index(t => t.ItemId)
                .Index(t => t.Partition_Id)
                .Index(t => t.Task_Id)
                .Index(t => t.Stage_Id)
                .Index(t => t.EventUser_Id)
                .Index(t => t.User_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
            CreateTable(
                "dbo.ProjectsFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FolderId = c.Int(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        FullPath = c.String(),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProjectsFolders", t => t.FolderId, cascadeDelete: false)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: false)
                .Index(t => t.FolderId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.ProjectsFolders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        ParentFolderId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProjectsFolders", t => t.ParentFolderId)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: false)
                .Index(t => t.ProjectId)
                .Index(t => t.ParentFolderId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        DeadLine = c.DateTime(nullable: false),
                        StateId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        IsPreFinish = c.Boolean(nullable: false),
                        IsFinish = c.Boolean(nullable: false),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                        Manager_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.Manager_Id)
                .ForeignKey("dbo.TasksStates", t => t.StateId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .Index(t => t.StateId)
                .Index(t => t.Creator_Id)
                .Index(t => t.Manager_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
            CreateTable(
                "dbo.Stages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        StateId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        IsPreFinish = c.Boolean(nullable: false),
                        IsFinish = c.Boolean(nullable: false),
                        Creator_Id = c.String(maxLength: 128),
                        Executor_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.TasksStates", t => t.StateId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.Executor_Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .Index(t => t.ProjectId)
                .Index(t => t.StateId)
                .Index(t => t.Creator_Id)
                .Index(t => t.Executor_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
            CreateTable(
                "dbo.Dependencies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TableId = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        FileDep_Id = c.Int(),
                        Task_Id = c.Int(),
                        TaskDep_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProjectsFiles", t => t.FileDep_Id)                
                .ForeignKey("dbo.Tasks", t => t.Task_Id)                
                .ForeignKey("dbo.TablesTypes", t => t.TableId, cascadeDelete: false)                
                .ForeignKey("dbo.Tasks", t => t.TaskDep_Id)
                .Index(t => t.TableId)
                .Index(t => t.ItemId)
                .Index(t => t.FileDep_Id)
                .Index(t => t.Task_Id)
                .Index(t => t.TaskDep_Id);
            
            CreateTable(
                "dbo.Partitions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StageId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeadLine = c.DateTime(nullable: false),
                        StateId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        IsPreFinish = c.Boolean(nullable: false),
                        IsFinish = c.Boolean(nullable: false),
                        Creator_Id = c.String(maxLength: 128),
                        Manager_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Manager_Id)
                .ForeignKey("dbo.Stages", t => t.StageId, cascadeDelete: false)
                .ForeignKey("dbo.TasksStates", t => t.StateId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .Index(t => t.StageId)
                .Index(t => t.StateId)
                .Index(t => t.Creator_Id)
                .Index(t => t.Manager_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
            CreateTable(
                "dbo.TasksStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PartitionId = c.Int(nullable: false),
                        IsPreFinish = c.Boolean(nullable: false),
                        IsFinish = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeadLine = c.DateTime(nullable: false),
                        StateId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                        Executor_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.Executor_Id)
                .ForeignKey("dbo.Partitions", t => t.PartitionId, cascadeDelete: false)
                .ForeignKey("dbo.TasksStates", t => t.StateId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .Index(t => t.PartitionId)
                .Index(t => t.StateId)
                .Index(t => t.Creator_Id)
                .Index(t => t.Executor_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
            CreateTable(
                "dbo.TablesTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TableName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventsTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "Name", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "Surname", c => c.String());
            AddColumn("dbo.AspNetUsers", "Fathername", c => c.String());
            AddColumn("dbo.AspNetUsers", "Birthday", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tasks", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Stages", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.Stages", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Projects", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.Projects", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Partitions", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.Partitions", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Comments", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "EventUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "EventId", "dbo.EventsTypes");
            DropForeignKey("dbo.Stages", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Stages", "Executor_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "Stage_Id", "dbo.Stages");
            DropForeignKey("dbo.Dependencies", "TaskDep_Id", "dbo.Tasks");
            
            DropForeignKey("dbo.Events", "TableId", "dbo.TablesTypes");
            DropForeignKey("dbo.Dependencies", "TableId", "dbo.TablesTypes");
            DropForeignKey("dbo.Comments", "TableId", "dbo.TablesTypes");
            
            DropForeignKey("dbo.Tasks", "StateId", "dbo.TasksStates");
            DropForeignKey("dbo.Tasks", "PartitionId", "dbo.Partitions");
            DropForeignKey("dbo.Tasks", "Executor_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "Task_Id", "dbo.Tasks");
            DropForeignKey("dbo.Dependencies", "Task_Id", "dbo.Tasks");
            DropForeignKey("dbo.Tasks", "Creator_Id", "dbo.AspNetUsers");
            
            DropForeignKey("dbo.Stages", "StateId", "dbo.TasksStates");
            DropForeignKey("dbo.Projects", "StateId", "dbo.TasksStates");
            DropForeignKey("dbo.Partitions", "StateId", "dbo.TasksStates");
            DropForeignKey("dbo.Partitions", "StageId", "dbo.Stages");
            DropForeignKey("dbo.Partitions", "Manager_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "Partition_Id", "dbo.Partitions");
            
            DropForeignKey("dbo.Partitions", "Creator_Id", "dbo.AspNetUsers");
            
            DropForeignKey("dbo.Dependencies", "FileDep_Id", "dbo.ProjectsFiles");
            DropForeignKey("dbo.Stages", "Creator_Id", "dbo.AspNetUsers");
            
            DropForeignKey("dbo.Projects", "Manager_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectsFolders", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectsFiles", "ProjectId", "dbo.Projects");
            
            DropForeignKey("dbo.Projects", "Creator_Id", "dbo.AspNetUsers");
            
            DropForeignKey("dbo.ProjectsFolders", "ParentFolderId", "dbo.ProjectsFolders");
            DropForeignKey("dbo.ProjectsFiles", "FolderId", "dbo.ProjectsFolders");
            
            DropIndex("dbo.Tasks", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Tasks", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Tasks", new[] { "Executor_Id" });
            DropIndex("dbo.Tasks", new[] { "Creator_Id" });
            DropIndex("dbo.Tasks", new[] { "StateId" });
            DropIndex("dbo.Tasks", new[] { "PartitionId" });
            DropIndex("dbo.Partitions", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Partitions", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Partitions", new[] { "Manager_Id" });
            DropIndex("dbo.Partitions", new[] { "Creator_Id" });
            DropIndex("dbo.Partitions", new[] { "StateId" });
            DropIndex("dbo.Partitions", new[] { "StageId" });
            DropIndex("dbo.Dependencies", new[] { "TaskDep_Id" });
            DropIndex("dbo.Dependencies", new[] { "Task_Id" });
            DropIndex("dbo.Dependencies", new[] { "FileDep_Id" });
            DropIndex("dbo.Dependencies", new[] { "ItemId" });
            DropIndex("dbo.Dependencies", new[] { "TableId" });
            DropIndex("dbo.Stages", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Stages", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Stages", new[] { "Executor_Id" });
            DropIndex("dbo.Stages", new[] { "Creator_Id" });
            DropIndex("dbo.Stages", new[] { "StateId" });
            DropIndex("dbo.Stages", new[] { "ProjectId" });
            DropIndex("dbo.Projects", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Projects", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Projects", new[] { "Manager_Id" });
            DropIndex("dbo.Projects", new[] { "Creator_Id" });
            DropIndex("dbo.Projects", new[] { "StateId" });
            DropIndex("dbo.ProjectsFolders", new[] { "ParentFolderId" });
            DropIndex("dbo.ProjectsFolders", new[] { "ProjectId" });
            DropIndex("dbo.ProjectsFiles", new[] { "ProjectId" });
            DropIndex("dbo.ProjectsFiles", new[] { "FolderId" });
            DropIndex("dbo.Events", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Events", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Events", new[] { "User_Id" });
            DropIndex("dbo.Events", new[] { "EventUser_Id" });
            DropIndex("dbo.Events", new[] { "Stage_Id" });
            DropIndex("dbo.Events", new[] { "Task_Id" });
            DropIndex("dbo.Events", new[] { "Partition_Id" });
            DropIndex("dbo.Events", new[] { "ItemId" });
            DropIndex("dbo.Events", new[] { "TableId" });
            DropIndex("dbo.Events", new[] { "EventId" });
            DropIndex("dbo.Comments", new[] { "User_Id" });
            DropIndex("dbo.Comments", new[] { "ItemId" });
            DropIndex("dbo.Comments", new[] { "TableId" });
            DropColumn("dbo.AspNetUsers", "Birthday");
            DropColumn("dbo.AspNetUsers", "Fathername");
            DropColumn("dbo.AspNetUsers", "Surname");
            DropColumn("dbo.AspNetUsers", "Name");
            DropTable("dbo.EventsTypes");
            DropTable("dbo.TablesTypes");
            DropTable("dbo.Tasks");
            DropTable("dbo.TasksStates");
            DropTable("dbo.Partitions");
            DropTable("dbo.Dependencies");
            DropTable("dbo.Stages");
            DropTable("dbo.Projects");
            DropTable("dbo.ProjectsFolders");
            DropTable("dbo.ProjectsFiles");
            DropTable("dbo.Events");
            DropTable("dbo.Comments");
        }
    }
}

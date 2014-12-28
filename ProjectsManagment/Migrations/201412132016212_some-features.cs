namespace ProjectsManagment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class somefeatures : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Events", "ItemId", "dbo.Comments");
            //DropForeignKey("dbo.Comments", "ItemId", "dbo.ProjectsFiles");
            //DropForeignKey("dbo.Comments", "ItemId", "dbo.Partitions");
            //DropForeignKey("dbo.Comments", "ItemId", "dbo.Projects");
            //DropForeignKey("dbo.Comments", "ItemId", "dbo.Stages");
            //DropForeignKey("dbo.Comments", "ItemId", "dbo.Tasks");
            //DropForeignKey("dbo.Events", "ItemId", "dbo.ProjectsFiles");
            //DropForeignKey("dbo.Events", "ItemId", "dbo.ProjectsFolders");
            //DropForeignKey("dbo.Events", "ItemId", "dbo.Projects");
            //DropForeignKey("dbo.Dependencies", "ItemId", "dbo.Partitions");
            //DropForeignKey("dbo.Dependencies", "ItemId", "dbo.Projects");
            //DropForeignKey("dbo.Dependencies", "ItemId", "dbo.Stages");
            //DropForeignKey("dbo.Dependencies", "ItemId", "dbo.Tasks");
            //DropIndex("dbo.Comments", new[] { "ItemId" });
            //DropIndex("dbo.Events", new[] { "ItemId" });
            //DropIndex("dbo.Dependencies", new[] { "ItemId" });
            RenameColumn(table: "dbo.Events", name: "Partition_Id", newName: "EventPartition_Id");
            RenameColumn(table: "dbo.Events", name: "Stage_Id", newName: "EventStage_Id");
            RenameColumn(table: "dbo.Events", name: "Task_Id", newName: "EventTask_Id");
            RenameIndex(table: "dbo.Events", name: "IX_Stage_Id", newName: "IX_EventStage_Id");
            RenameIndex(table: "dbo.Events", name: "IX_Task_Id", newName: "IX_EventTask_Id");
            RenameIndex(table: "dbo.Events", name: "IX_Partition_Id", newName: "IX_EventPartition_Id");
            AddColumn("dbo.Comments", "Bad", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comments", "File_Id", c => c.Int());
            AddColumn("dbo.Comments", "Project_Id", c => c.Int());
            AddColumn("dbo.Comments", "Partition_Id", c => c.Int());
            AddColumn("dbo.Comments", "Stage_Id", c => c.Int());
            AddColumn("dbo.Comments", "Task_Id", c => c.Int());
            AddColumn("dbo.Events", "EventComment_Id", c => c.Int());
            AddColumn("dbo.Events", "EventFile_Id", c => c.Int());
            AddColumn("dbo.Events", "EventFolder_Id", c => c.Int());
            AddColumn("dbo.Events", "EventProject_Id", c => c.Int());
            AddColumn("dbo.Dependencies", "Partition_Id", c => c.Int());
            AddColumn("dbo.Dependencies", "Project_Id", c => c.Int());
            AddColumn("dbo.Dependencies", "Stage_Id", c => c.Int());
            AddColumn("dbo.Dependencies", "Task_Id1", c => c.Int());
            CreateIndex("dbo.Comments", "File_Id");
            CreateIndex("dbo.Comments", "Project_Id");
            CreateIndex("dbo.Comments", "Partition_Id");
            CreateIndex("dbo.Comments", "Stage_Id");
            CreateIndex("dbo.Comments", "Task_Id");
            CreateIndex("dbo.Events", "EventComment_Id");
            CreateIndex("dbo.Events", "EventFile_Id");
            CreateIndex("dbo.Events", "EventFolder_Id");
            CreateIndex("dbo.Events", "EventProject_Id");
            CreateIndex("dbo.Dependencies", "Partition_Id");
            CreateIndex("dbo.Dependencies", "Project_Id");
            CreateIndex("dbo.Dependencies", "Stage_Id");
            CreateIndex("dbo.Dependencies", "Task_Id1");
            AddForeignKey("dbo.Events", "EventComment_Id", "dbo.Comments", "Id");
            AddForeignKey("dbo.Comments", "File_Id", "dbo.ProjectsFiles", "Id");
            AddForeignKey("dbo.Comments", "Partition_Id", "dbo.Partitions", "Id");
            AddForeignKey("dbo.Comments", "Project_Id", "dbo.Projects", "Id");
            AddForeignKey("dbo.Comments", "Stage_Id", "dbo.Stages", "Id");
            AddForeignKey("dbo.Comments", "Task_Id", "dbo.Tasks", "Id");
            AddForeignKey("dbo.Events", "EventFile_Id", "dbo.ProjectsFiles", "Id");
            AddForeignKey("dbo.Events", "EventFolder_Id", "dbo.ProjectsFolders", "Id");
            AddForeignKey("dbo.Events", "EventProject_Id", "dbo.Projects", "Id");
            AddForeignKey("dbo.Dependencies", "Partition_Id", "dbo.Partitions", "Id");
            AddForeignKey("dbo.Dependencies", "Project_Id", "dbo.Projects", "Id");
            AddForeignKey("dbo.Dependencies", "Stage_Id", "dbo.Stages", "Id");
            AddForeignKey("dbo.Dependencies", "Task_Id1", "dbo.Tasks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Dependencies", "Task_Id1", "dbo.Tasks");
            DropForeignKey("dbo.Dependencies", "Stage_Id", "dbo.Stages");
            DropForeignKey("dbo.Dependencies", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.Dependencies", "Partition_Id", "dbo.Partitions");
            DropForeignKey("dbo.Events", "EventProject_Id", "dbo.Projects");
            DropForeignKey("dbo.Events", "EventFolder_Id", "dbo.ProjectsFolders");
            DropForeignKey("dbo.Events", "EventFile_Id", "dbo.ProjectsFiles");
            DropForeignKey("dbo.Comments", "Task_Id", "dbo.Tasks");
            DropForeignKey("dbo.Comments", "Stage_Id", "dbo.Stages");
            DropForeignKey("dbo.Comments", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.Comments", "Partition_Id", "dbo.Partitions");
            DropForeignKey("dbo.Comments", "File_Id", "dbo.ProjectsFiles");
            DropForeignKey("dbo.Events", "EventComment_Id", "dbo.Comments");
            DropIndex("dbo.Dependencies", new[] { "Task_Id1" });
            DropIndex("dbo.Dependencies", new[] { "Stage_Id" });
            DropIndex("dbo.Dependencies", new[] { "Project_Id" });
            DropIndex("dbo.Dependencies", new[] { "Partition_Id" });
            DropIndex("dbo.Events", new[] { "EventProject_Id" });
            DropIndex("dbo.Events", new[] { "EventFolder_Id" });
            DropIndex("dbo.Events", new[] { "EventFile_Id" });
            DropIndex("dbo.Events", new[] { "EventComment_Id" });
            DropIndex("dbo.Comments", new[] { "Task_Id" });
            DropIndex("dbo.Comments", new[] { "Stage_Id" });
            DropIndex("dbo.Comments", new[] { "Partition_Id" });
            DropIndex("dbo.Comments", new[] { "Project_Id" });
            DropIndex("dbo.Comments", new[] { "File_Id" });
            DropColumn("dbo.Dependencies", "Task_Id1");
            DropColumn("dbo.Dependencies", "Stage_Id");
            DropColumn("dbo.Dependencies", "Project_Id");
            DropColumn("dbo.Dependencies", "Partition_Id");
            DropColumn("dbo.Events", "EventProject_Id");
            DropColumn("dbo.Events", "EventFolder_Id");
            DropColumn("dbo.Events", "EventFile_Id");
            DropColumn("dbo.Events", "EventComment_Id");
            DropColumn("dbo.Comments", "Task_Id");
            DropColumn("dbo.Comments", "Stage_Id");
            DropColumn("dbo.Comments", "Partition_Id");
            DropColumn("dbo.Comments", "Project_Id");
            DropColumn("dbo.Comments", "File_Id");
            DropColumn("dbo.Comments", "Bad");
            RenameIndex(table: "dbo.Events", name: "IX_EventPartition_Id", newName: "IX_Partition_Id");
            RenameIndex(table: "dbo.Events", name: "IX_EventTask_Id", newName: "IX_Task_Id");
            RenameIndex(table: "dbo.Events", name: "IX_EventStage_Id", newName: "IX_Stage_Id");
            RenameColumn(table: "dbo.Events", name: "EventTask_Id", newName: "Task_Id");
            RenameColumn(table: "dbo.Events", name: "EventStage_Id", newName: "Stage_Id");
            RenameColumn(table: "dbo.Events", name: "EventPartition_Id", newName: "Partition_Id");
            CreateIndex("dbo.Dependencies", "ItemId");
            CreateIndex("dbo.Events", "ItemId");
            CreateIndex("dbo.Comments", "ItemId");
            //AddForeignKey("dbo.Dependencies", "ItemId", "dbo.Tasks", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Dependencies", "ItemId", "dbo.Stages", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Dependencies", "ItemId", "dbo.Projects", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Dependencies", "ItemId", "dbo.Partitions", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Events", "ItemId", "dbo.Projects", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Events", "ItemId", "dbo.ProjectsFolders", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Events", "ItemId", "dbo.ProjectsFiles", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Comments", "ItemId", "dbo.Tasks", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Comments", "ItemId", "dbo.Stages", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Comments", "ItemId", "dbo.Projects", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Comments", "ItemId", "dbo.Partitions", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Comments", "ItemId", "dbo.ProjectsFiles", "Id", cascadeDelete: true);
            //AddForeignKey("dbo.Events", "ItemId", "dbo.Comments", "Id", cascadeDelete: true);
        }
    }
}

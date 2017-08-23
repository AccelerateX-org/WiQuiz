namespace WIQuest.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Training : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrainingExercises",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsAvailabe = c.Boolean(nullable: false),
                        AvailableFrom = c.DateTime(),
                        AvailableUntil = c.DateTime(),
                        Quiz_Id = c.Guid(),
                        TrainingSession_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.Quiz_Id)
                .ForeignKey("dbo.TrainingSessions", t => t.TrainingSession_Id)
                .Index(t => t.Quiz_Id)
                .Index(t => t.TrainingSession_Id);
            
            CreateTable(
                "dbo.TrainingSessions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        TrainingPlan_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingPlans", t => t.TrainingPlan_Id)
                .Index(t => t.TrainingPlan_Id);
            
            CreateTable(
                "dbo.TrainingPlans",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrainingGroups",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsPublic = c.Boolean(nullable: false),
                        AccessToken = c.String(),
                        TrainingPlan_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingPlans", t => t.TrainingPlan_Id)
                .Index(t => t.TrainingPlan_Id);
            
            CreateTable(
                "dbo.TrainingGroupSubscriptions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Userid = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                        TrainingGroup_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingGroups", t => t.TrainingGroup_Id)
                .Index(t => t.TrainingGroup_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainingSessions", "TrainingPlan_Id", "dbo.TrainingPlans");
            DropForeignKey("dbo.TrainingGroups", "TrainingPlan_Id", "dbo.TrainingPlans");
            DropForeignKey("dbo.TrainingGroupSubscriptions", "TrainingGroup_Id", "dbo.TrainingGroups");
            DropForeignKey("dbo.TrainingExercises", "TrainingSession_Id", "dbo.TrainingSessions");
            DropForeignKey("dbo.TrainingExercises", "Quiz_Id", "dbo.Quizs");
            DropIndex("dbo.TrainingGroupSubscriptions", new[] { "TrainingGroup_Id" });
            DropIndex("dbo.TrainingGroups", new[] { "TrainingPlan_Id" });
            DropIndex("dbo.TrainingSessions", new[] { "TrainingPlan_Id" });
            DropIndex("dbo.TrainingExercises", new[] { "TrainingSession_Id" });
            DropIndex("dbo.TrainingExercises", new[] { "Quiz_Id" });
            DropTable("dbo.TrainingGroupSubscriptions");
            DropTable("dbo.TrainingGroups");
            DropTable("dbo.TrainingPlans");
            DropTable("dbo.TrainingSessions");
            DropTable("dbo.TrainingExercises");
        }
    }
}

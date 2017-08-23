namespace WIQuest.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Practice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrainingPractices",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Exercise_Id = c.Guid(),
                        Game_Id = c.Guid(),
                        Subscription_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingExercises", t => t.Exercise_Id)
                .ForeignKey("dbo.QuizGames", t => t.Game_Id)
                .ForeignKey("dbo.TrainingGroupSubscriptions", t => t.Subscription_Id)
                .Index(t => t.Exercise_Id)
                .Index(t => t.Game_Id)
                .Index(t => t.Subscription_Id);
            
            CreateTable(
                "dbo.TrainingSchedules",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        IsFromRestricted = c.Boolean(nullable: false),
                        IsUntilRestricted = c.Boolean(nullable: false),
                        AvailableFrom = c.DateTime(),
                        AvailableUntil = c.DateTime(),
                        Exercise_Id = c.Guid(),
                        Group_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingExercises", t => t.Exercise_Id)
                .ForeignKey("dbo.TrainingGroups", t => t.Group_Id)
                .Index(t => t.Exercise_Id)
                .Index(t => t.Group_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainingSchedules", "Group_Id", "dbo.TrainingGroups");
            DropForeignKey("dbo.TrainingSchedules", "Exercise_Id", "dbo.TrainingExercises");
            DropForeignKey("dbo.TrainingPractices", "Subscription_Id", "dbo.TrainingGroupSubscriptions");
            DropForeignKey("dbo.TrainingPractices", "Game_Id", "dbo.QuizGames");
            DropForeignKey("dbo.TrainingPractices", "Exercise_Id", "dbo.TrainingExercises");
            DropIndex("dbo.TrainingSchedules", new[] { "Group_Id" });
            DropIndex("dbo.TrainingSchedules", new[] { "Exercise_Id" });
            DropIndex("dbo.TrainingPractices", new[] { "Subscription_Id" });
            DropIndex("dbo.TrainingPractices", new[] { "Game_Id" });
            DropIndex("dbo.TrainingPractices", new[] { "Exercise_Id" });
            DropTable("dbo.TrainingSchedules");
            DropTable("dbo.TrainingPractices");
        }
    }
}

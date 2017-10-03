namespace WIQuest.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionAnswers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Text = c.String(),
                        IsCorrect = c.Boolean(nullable: false),
                        Reihenfolge = c.Int(nullable: false),
                        Image_Id = c.Guid(),
                        Question_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BinaryStorages", t => t.Image_Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .Index(t => t.Image_Id)
                .Index(t => t.Question_Id);
            
            CreateTable(
                "dbo.BinaryStorages",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ImageFileType = c.String(),
                        ImageData = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ExternalId = c.String(),
                        Title = c.String(),
                        Text = c.String(),
                        Level = c.Int(nullable: false),
                        Reihenfolge = c.Int(nullable: false),
                        Category_Id = c.Guid(),
                        Image_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuestionCategories", t => t.Category_Id)
                .ForeignKey("dbo.BinaryStorages", t => t.Image_Id)
                .Index(t => t.Category_Id)
                .Index(t => t.Image_Id);
            
            CreateTable(
                "dbo.QuestionCategories",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ShortName = c.String(),
                        Name = c.String(),
                        Reihenfolge = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuizQuestions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Question_Id = c.Guid(),
                        Section_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .ForeignKey("dbo.QuizSections", t => t.Section_Id)
                .Index(t => t.Question_Id)
                .Index(t => t.Section_Id);
            
            CreateTable(
                "dbo.QuizSections",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Quiz_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.Quiz_Id)
                .Index(t => t.Quiz_Id);
            
            CreateTable(
                "dbo.Quizs",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuestLogs",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FirstView = c.DateTime(nullable: false),
                        LastAction = c.DateTime(),
                        ViewCount = c.Int(nullable: false),
                        MinViewDuration = c.Int(nullable: false),
                        MaxViewDuration = c.Int(nullable: false),
                        Answer_Id = c.Guid(),
                        Question_Id = c.Guid(),
                        User_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuestionAnswers", t => t.Answer_Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Answer_Id)
                .Index(t => t.Question_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId  = c.String(nullable: true),
                        Geschlecht = c.Int(nullable: false),
                        Altersgruppe = c.Int(nullable: false),
                        Hochschulzugangsberechtigung = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestLogs", "User_Id", "dbo.Users");
            DropForeignKey("dbo.QuestLogs", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.QuestLogs", "Answer_Id", "dbo.QuestionAnswers");
            DropForeignKey("dbo.QuizSections", "Quiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.QuizQuestions", "Section_Id", "dbo.QuizSections");
            DropForeignKey("dbo.QuizQuestions", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.Questions", "Image_Id", "dbo.BinaryStorages");
            DropForeignKey("dbo.Questions", "Category_Id", "dbo.QuestionCategories");
            DropForeignKey("dbo.QuestionAnswers", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.QuestionAnswers", "Image_Id", "dbo.BinaryStorages");
            DropIndex("dbo.QuestLogs", new[] { "User_Id" });
            DropIndex("dbo.QuestLogs", new[] { "Question_Id" });
            DropIndex("dbo.QuestLogs", new[] { "Answer_Id" });
            DropIndex("dbo.QuizSections", new[] { "Quiz_Id" });
            DropIndex("dbo.QuizQuestions", new[] { "Section_Id" });
            DropIndex("dbo.QuizQuestions", new[] { "Question_Id" });
            DropIndex("dbo.Questions", new[] { "Image_Id" });
            DropIndex("dbo.Questions", new[] { "Category_Id" });
            DropIndex("dbo.QuestionAnswers", new[] { "Question_Id" });
            DropIndex("dbo.QuestionAnswers", new[] { "Image_Id" });
            DropTable("dbo.Users");
            DropTable("dbo.QuestLogs");
            DropTable("dbo.Quizs");
            DropTable("dbo.QuizSections");
            DropTable("dbo.QuizQuestions");
            DropTable("dbo.QuestionCategories");
            DropTable("dbo.Questions");
            DropTable("dbo.BinaryStorages");
            DropTable("dbo.QuestionAnswers");
        }
    }
}

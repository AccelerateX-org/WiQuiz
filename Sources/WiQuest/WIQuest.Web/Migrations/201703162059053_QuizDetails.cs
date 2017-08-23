namespace WIQuest.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuizDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quizs", "IsRandomByQuestion", c => c.Boolean(nullable: false));
            AddColumn("dbo.Quizs", "IsRandomByAnswer", c => c.Boolean(nullable: false));
            AddColumn("dbo.Quizs", "ShowExplanations", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quizs", "ShowExplanations");
            DropColumn("dbo.Quizs", "IsRandomByAnswer");
            DropColumn("dbo.Quizs", "IsRandomByQuestion");
        }
    }
}

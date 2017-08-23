namespace WIQuest.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuizLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quizs", "Level", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quizs", "Level");
        }
    }
}

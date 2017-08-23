namespace WIQuest.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Explanations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QuestionAnswers", "Explanation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.QuestionAnswers", "Explanation");
        }
    }
}

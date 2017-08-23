namespace WIQuest.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Publishing : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quizs", "IsPubslished", c => c.Boolean(nullable: false));

            Sql("UPDATE dbo.Quizs SET IsPubslished  = 'True'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quizs", "IsPubslished");
        }
    }
}

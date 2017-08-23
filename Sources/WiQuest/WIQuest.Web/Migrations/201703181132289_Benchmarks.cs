namespace WIQuest.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Benchmarks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrainingBenchmarkRanges",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LowerBorder = c.Double(nullable: false),
                        UpperBorder = c.Double(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Feedback = c.String(),
                        Benchmark_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingBenchmarks", t => t.Benchmark_Id)
                .Index(t => t.Benchmark_Id);
            
            CreateTable(
                "dbo.TrainingBenchmarks",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TrainingExercises", "Benchmark_Id", c => c.Guid());
            AddColumn("dbo.TrainingPlans", "IsPublic", c => c.Boolean(nullable: false));
            AddColumn("dbo.TrainingPlans", "Benchmark_Id", c => c.Guid());
            AddColumn("dbo.TrainingSessions", "Benchmark_Id", c => c.Guid());
            CreateIndex("dbo.TrainingExercises", "Benchmark_Id");
            CreateIndex("dbo.TrainingPlans", "Benchmark_Id");
            CreateIndex("dbo.TrainingSessions", "Benchmark_Id");
            AddForeignKey("dbo.TrainingExercises", "Benchmark_Id", "dbo.TrainingBenchmarks", "Id");
            AddForeignKey("dbo.TrainingPlans", "Benchmark_Id", "dbo.TrainingBenchmarks", "Id");
            AddForeignKey("dbo.TrainingSessions", "Benchmark_Id", "dbo.TrainingBenchmarks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainingSessions", "Benchmark_Id", "dbo.TrainingBenchmarks");
            DropForeignKey("dbo.TrainingPlans", "Benchmark_Id", "dbo.TrainingBenchmarks");
            DropForeignKey("dbo.TrainingExercises", "Benchmark_Id", "dbo.TrainingBenchmarks");
            DropForeignKey("dbo.TrainingBenchmarkRanges", "Benchmark_Id", "dbo.TrainingBenchmarks");
            DropIndex("dbo.TrainingSessions", new[] { "Benchmark_Id" });
            DropIndex("dbo.TrainingPlans", new[] { "Benchmark_Id" });
            DropIndex("dbo.TrainingExercises", new[] { "Benchmark_Id" });
            DropIndex("dbo.TrainingBenchmarkRanges", new[] { "Benchmark_Id" });
            DropColumn("dbo.TrainingSessions", "Benchmark_Id");
            DropColumn("dbo.TrainingPlans", "Benchmark_Id");
            DropColumn("dbo.TrainingPlans", "IsPublic");
            DropColumn("dbo.TrainingExercises", "Benchmark_Id");
            DropTable("dbo.TrainingBenchmarks");
            DropTable("dbo.TrainingBenchmarkRanges");
        }
    }
}

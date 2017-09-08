public class BuildAdjustments 
{
    private static ICakeContext _context;
    
    private static void supressBreakingBuild() {
        BuildParameters.Tasks.DupFinderTask.ContinueOnError();
        BuildParameters.Tasks.InspectCodeTask.ContinueOnError();
    }

    private static void reportOnFailingTest() {
        BuildParameters.Tasks.TestxUnitTask
            .OnError(exception =>
            {
                var reportSubDir = "/Report";
                var unitTestZip = "UnitTestReport.zip";
                var codeCoverageZip = "CodeCoverageReport.zip";
                
                // Generate Unit Test Report
                var xUnitTestReport = BuildParameters.Paths.Directories.xUnitTestResults + reportSubDir;
                _context.EnsureDirectoryExists(xUnitTestReport);
                _context.ReportUnit(BuildParameters.Paths.Directories.xUnitTestResults, xUnitTestReport, new ReportUnitSettings());

                // Generate Code Coverage Report
                var codeCoverageReport = BuildParameters.Paths.Directories.TestCoverage + reportSubDir;
                _context.ReportGenerator(BuildParameters.Paths.Files.TestCoverageOutputFilePath, codeCoverageReport);

                // Zip Report Files
                _context.Information("Zipping Unit Test Results...");
                _context.Zip(xUnitTestReport, BuildParameters.Paths.Directories.xUnitTestResults.CombineWithFilePath(unitTestZip));

                _context.Information("Zipping Code Coverage Results...");
                _context.Zip(codeCoverageReport, BuildParameters.Paths.Directories.TestCoverage.CombineWithFilePath(codeCoverageZip));             

                // Upload to AppVeyor if Build System
                if(BuildParameters.IsRunningOnAppVeyor)
                {
                    if (FileExists(unitTestZip)) 
                    {
                        _context.AppVeyor.UploadArtifact(unitTestZip);
                    }
                    if (FileExists(codeCoverageZip)) 
                    {
                        _context.AppVeyor.UploadArtifact(codeCoverageZip);
                    }
                } 
            });
    }

    public static void Set(ICakeContext context) {
        _context = context;
        supressBreakingBuild();
        reportOnFailingTest();
    }
}
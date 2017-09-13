///////////////////////////////////////////////////////////////////////////////
// Supress breaking the Build
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.DupFinderTask.ContinueOnError();
BuildParameters.Tasks.InspectCodeTask.ContinueOnError();

///////////////////////////////////////////////////////////////////////////////
// Generate reports of processed test results
// Workaround: Reports nevertheless tests failed
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.TestxUnitTask
    .Does(() => {
        // TODO
	})
    .OnError(exception => {
        var reportSubDir = "/Report";
        var unitTestZip = BuildParameters.Paths.Directories.xUnitTestResults.CombineWithFilePath("UnitTestReport.zip");
        var codeCoverageZip = BuildParameters.Paths.Directories.TestCoverage.CombineWithFilePath("CodeCoverageReport.zip");

        // Generate Unit Test Report
        var xUnitTestReport = BuildParameters.Paths.Directories.xUnitTestResults + reportSubDir;
        EnsureDirectoryExists(xUnitTestReport);
        ReportUnit(BuildParameters.Paths.Directories.xUnitTestResults, xUnitTestReport, new ReportUnitSettings());

        // Generate Code Coverage Report
        var codeCoverageReport = BuildParameters.Paths.Directories.TestCoverage + reportSubDir;
        ReportGenerator(BuildParameters.Paths.Files.TestCoverageOutputFilePath, codeCoverageReport);
  
        Information("Zipping Unit Test Results...");
        Zip(xUnitTestReport, unitTestZip);

        Information("Zipping Code Coverage Results...");
        Zip(codeCoverageReport, codeCoverageZip);

        // Upload to AppVeyor if Build System
        if (BuildParameters.IsRunningOnAppVeyor) 
        {
            if (FileExists(unitTestZip)) 
            {
                AppVeyor.UploadArtifact(unitTestZip);
            }
            if (FileExists(codeCoverageZip)) 
            {
                AppVeyor.UploadArtifact(codeCoverageZip);
            }
        }
 });

// Zip Report Files
public void ZipReportFiles()
{
    // TODO
}
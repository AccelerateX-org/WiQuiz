///////////////////////////////////////////////////////////////////////////////
// Supress breaking the Build
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.DupFinderTask.ContinueOnError();
BuildParameters.Tasks.InspectCodeTask.ContinueOnError();

///////////////////////////////////////////////////////////////////////////////
// Generate reports of processed test results
// Workaround: Reports nevertheless tests are failing
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.TestNUnitTask
    .Does(() => {
        if (BuildParameters.IsRunningOnAppVeyor) 
        {
            BuildSystem.AppVeyor.UploadTestResults(BuildParameters.Paths.Directories.NUnitTestResults, AppVeyorTestResultsType.NUnit3);
        }
	})
    .OnError(exception => {
        var reportSubDir = "/Report";
        var unitTestZip = BuildParameters.Paths.Directories.NUnitTestResults.CombineWithFilePath("UnitTestReport.zip");
        var codeCoverageZip = BuildParameters.Paths.Directories.TestCoverage.CombineWithFilePath("CodeCoverageReport.zip");

        // Generate Unit Test Report
        var NUnitTestReport = BuildParameters.Paths.Directories.NUnitTestResults + reportSubDir;
        EnsureDirectoryExists(NUnitTestReport);
        ReportUnit(BuildParameters.Paths.Directories.NUnitTestResults, NUnitTestReport, new ReportUnitSettings());

        // Generate Code Coverage Report
        var codeCoverageReport = BuildParameters.Paths.Directories.TestCoverage + reportSubDir;
        ReportGenerator(BuildParameters.Paths.Files.TestCoverageOutputFilePath, codeCoverageReport);
  
        Information("Zipping Unit Test Results...");
        Zip(NUnitTestReport, unitTestZip);

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

        throw exception;
 });

// Zip Report Files
public void ZipReportFiles()
{
    // TODO
}
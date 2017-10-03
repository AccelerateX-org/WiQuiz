///////////////////////////////////////////////////////////////////////////////
// Supress breaking the Build
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.DupFinderTask.ContinueOnError();
BuildParameters.Tasks.InspectCodeTask.ContinueOnError();

///////////////////////////////////////////////////////////////////////////////
// Generate reports of processed test results
// Workaround: Reports nevertheless tests are failing
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.TestNUnitTask.Task.Actions.Clear();
BuildParameters.Tasks.TestNUnitTask
    .Does(() => RequireTool(NUnitTool, () => {
        EnsureDirectoryExists(BuildParameters.Paths.Directories.NUnitTestResults);

        OpenCover(tool => {
            tool.NUnit3(GetFiles(BuildParameters.Paths.Directories.PublishedNUnitTests + (BuildParameters.TestFilePattern ?? "/**/*Tests.dll")), new NUnit3Settings {
                NoResults = false,
                Work = BuildParameters.Paths.Directories.NUnitTestResults
            });
        },
        BuildParameters.Paths.Files.TestCoverageOutputFilePath,
        new OpenCoverSettings { ReturnTargetCodeOffset = 0 }
            .WithFilter(ToolSettings.TestCoverageFilter)
            .ExcludeByAttribute(ToolSettings.TestCoverageExcludeByAttribute)
            .ExcludeByFile(ToolSettings.TestCoverageExcludeByFile));

        if(BuildParameters.IsRunningOnAppVeyor)
        {
            // Upload artifact to AppVeyor.
            AppVeyor.UploadTestResults(BuildParameters.Paths.Directories.NUnitTestResults + "/TestResult.xml", AppVeyorTestResultsType.NUnit3);
        }    

        // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
        ReportGenerator(BuildParameters.Paths.Files.TestCoverageOutputFilePath, BuildParameters.Paths.Directories.TestCoverage);
	}))
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
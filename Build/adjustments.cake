BuildParameters.Tasks.DupFinderTask.ContinueOnError();
BuildParameters.Tasks.InspectCodeTask.ContinueOnError();

BuildParameters.Tasks.TestxUnitTask
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

        // Zip Report Files
        Information("Zipping Unit Test Results...");
        Zip(xUnitTestReport, unitTestZip);

        Information("Zipping Code Coverage Results...");
        Zip(codeCoverageReport, codeCoverageZip);

        // Upload to AppVeyor if Build System
        if (BuildParameters.IsRunningOnAppVeyor) {
            if (FileExists(unitTestZip)) {
                AppVeyor.UploadArtifact(unitTestZip);
            }
            if (FileExists(codeCoverageZip)) {
                AppVeyor.UploadArtifact(codeCoverageZip);
            }
        }
 });

#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./Sources",
                            title: "WIQuest",
                            repositoryOwner: "AccelerateX-org",
                            repositoryName: "WiQuiz",
                            appVeyorAccountName: "AccelerateX",
                            shouldRunDupFinder: false,
                            shouldRunInspectCode: false);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            testCoverageFilter: "+[WIQuest*]* -[WIQuest*.Test]*");

BuildParameters.Tasks.DupFinderTask.ContinueOnError();
BuildParameters.Tasks.InspectCodeTask.ContinueOnError();

BuildParameters.Tasks.TestxUnitTask
    .OnError(exception =>
    {
        ReportUnit(BuildParameters.Paths.Directories.xUnitTestResults, BuildParameters.Paths.Directories.xUnitTestResults, new ReportUnitSettings());

        ReportGenerator(BuildParameters.Paths.Files.TestCoverageOutputFilePath, BuildParameters.Paths.Directories.TestCoverage);
    });

Build.Run();
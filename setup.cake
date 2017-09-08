#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./Sources",
                            title: "WIQuest",
                            repositoryOwner: "AccelerateX-org",
                            repositoryName: "WiQuiz",
                            appVeyorAccountName: "AccelerateX");

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context);

BuildParameters.Tasks.DupFinderTask.ContinueOnError();
BuildParameters.Tasks.InspectCodeTask.ContinueOnError();
BuildParameters.Tasks.TestxUnitTask.ContinueOnError();

Build.Run();
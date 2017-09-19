#load "nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease&version=0.3.0-unstable0278"

#addin "nuget:?package=Octopus.Client&version=4.22.1"

#tool "nuget:?package=OctopusTools&version=4.22.1"

#load "./Build/rps.cake"
#load "./Build/targets.cake"
#load "./Build/adjustments.cake"
#load "./Build/package.cake"
#load "./Build/deploy.cake"

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./Sources",
                            integrationTestScriptPath: ".", // Workaround: NULL Exception
                            title: "WIQuest",
                            repositoryOwner: "wpankratz",
                            repositoryName: "WiQuiz",
                            appVeyorAccountName: "wpankratz",
                            shouldExecuteGitLink: false,
                            shouldRunDupFinder: false,
                            shouldRunInspectCode: false);
                            
BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            testCoverageFilter: "+[WIQuest*]* -[WIQuest*.Tests]*");

RPS.Init(context: Context);

Build.Run();
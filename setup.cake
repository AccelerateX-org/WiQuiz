#load "nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease&version=0.3.0-unstable0278"

#addin "nuget:?package=Octopus.Client&version=4.22.1"
#tool "nuget:?package=OctopusTools&version=4.22.1"

#load "./Build/rps.cake"
#load "./Build/targets.cake"
#load "./Build/adjustments.cake"
#load "./Build/package.cake"
#load "./Build/deploy.cake"
#load "./Build/uat.cake"

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./Sources",
                            integrationTestScriptPath: ".", // Workaround: NULL Exception
                            testFilePattern: "/**/*.Tests.dll",
                            title: "WIQuest",
                            repositoryOwner: "AccelerateX-org",
                            repositoryName: "WiQuiz",
                            appVeyorAccountName: "AccelerateX",
                            shouldExecuteGitLink: false,
                            shouldRunDupFinder: false,
                            shouldRunInspectCode: false);
                            
BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            testCoverageFilter: "+[WIQuest*]* -[WIQuest*.Tests]* -[WIQuest*.UaTests]*");

RPS.Init(context: Context);

Build.Run();

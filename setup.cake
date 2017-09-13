#load "nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease"
#load "./Build/targets.cake"
#load "./Build/adjustments.cake"
#load "./Build/package.cake"
#load "./Build/deploy.cake"

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./Sources",
                            title: "WIQuest",
                            repositoryOwner: "wpankratz",
                            repositoryName: "WiQuiz",
                            appVeyorAccountName: "wpankratz",
                            shouldRunDupFinder: false,
                            shouldRunInspectCode: false);
                            
BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            testCoverageFilter: "+[WIQuest*]* -[WIQuest*.Tests]*");

ScmRepository.setRepositoryDetails();                          

Build.Run();
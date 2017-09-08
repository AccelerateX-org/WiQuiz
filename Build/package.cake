///////////////////////////////////////////////////////////////////////////////
// Octopus Deployment Package via MSBuild Task
///////////////////////////////////////////////////////////////////////////////

// Clear Create-NuGet-Package actions
BuildParameters.Tasks.CreateNuGetPackageTask.Task.Actions.Clear();

BuildParameters.Tasks.CreateNuGetPackageTask
    .IsDependentOn("Build")
    .IsDependentOn("Test")
	.Does(() => {
		Information("Do something...");
	});
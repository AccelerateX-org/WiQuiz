Task("Developer-Build")
    .IsDependentOn("Build")
    .IsDependentOn("Test-xUnit")
    .IsDependentOn("Octopus-Packaging");



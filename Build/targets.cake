Task("Developer-Build")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("AppVeyor");



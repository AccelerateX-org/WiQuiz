Task("DevBuild")
    .IsDependentOn("Build")
    .IsDependentOn("Octopus-Packaging")
    .IsDependentOn("Octopus-Deployment");

Task("KpiBuild")
    .IsDependentOn("Build")
    .IsDependentOn("Test-NUnit")
    .IsDependentOn("Upload-Coveralls-Report")
    .IsDependentOn("Octopus-Packaging")
    .IsDependentOn("Octopus-Deployment");

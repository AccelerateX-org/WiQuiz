Task("DevBuild")
    .IsDependentOn("Build")
    .IsDependentOn("Octopus-Packaging")
    .IsDependentOn("Octopus-Deployment");

Task("PrBuild")
    .IsDependentOn("Build")
    .IsDependentOn("Test-NUnit");

Task("KpiBuild")
    .IsDependentOn("Build")
    .IsDependentOn("Test-NUnit")
    .IsDependentOn("Octopus-Packaging")
    .IsDependentOn("Octopus-Deployment");

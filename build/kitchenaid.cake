public static class KitchenAid 
{
    public static void cleanOutputDirectories(ICakeContext ctx, CheeseCake parameters, IReadOnlyCollection<SolutionProject> projectsToClean) {
        if (ctx == null) 
        {
            throw new ArgumentNullException("Missing context @ KitchenAid.cleanOutputDirectories()");
        }   
        foreach(var project in projectsToClean) {
		
            if (project.Type != "{2150E333-8FDC-42A3-9474-1A3956D46DE8}") {
                
            ctx.Information("Cleaning {0}", project.Path);
                
            var dir = project.Path.GetDirectory();
            ctx.CleanDirectories(dir + "/bin");
            ctx.CleanDirectories(dir + "/obj");
		
		    }
	    }

        ctx.Information("Cleaning {0}", parameters.OutputPath);
        ctx.CleanDirectories(parameters.OutputPath);

    }
}
#load "nuget:?package=Rocket.Surgery.Cake.Library&version=0.9.3";

// Default the target to "Default" if one was not specified.
var target = Argument("target", "Default");

Task("Submodules")
    .Does(() => {
        StartProcess("git", "submodule update --init --recursive");
    });

Task("Embed MediatR")
    .Does(() => {
        foreach (var file in GetFiles("submodules/**/*.cs"))
        {
            var content = System.IO.File.ReadAllText(file.FullPath);
            if (content.IndexOf("namespace MediatR") > -1 || content.IndexOf("using MediatR") > -1)
            {
                System.IO.File.WriteAllText(file.FullPath, content
                    .Replace("namespace MediatR", "namespace OmniSharp.Extensions.Embedded.MediatR")
                    .Replace("using MediatR", "using OmniShqarp.Extensions.Embedded.MediatR")
                );
            }
        }
    });

Task("Default")
    .IsDependentOn("Submodules")
    .IsDependentOn("Embed MediatR")
    .IsDependentOn("dotnetcore");

RunTarget(target);

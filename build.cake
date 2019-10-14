#load "nuget:?package=Rocket.Surgery.Cake.Library&version=0.10.1";

Task("Default")
    .IsDependentOn("dotnetcore");

RunTarget(Target);

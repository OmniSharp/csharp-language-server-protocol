#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=xunit.runner.console"


var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var artifacts = "./artifacts";

Task("Clean")
    .Does(() =>
{
    EnsureDirectoryExists(artifacts);
    CleanDirectory(artifacts);
});

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore();
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    foreach (var project in GetFiles("**/*.csproj"))
        DotNetCoreBuild(project.FullPath);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{

});

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

RunTarget(target);

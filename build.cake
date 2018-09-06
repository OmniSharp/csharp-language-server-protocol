#tool "nuget:?package=GitVersion.CommandLine&prerelease&version=4.0.0-beta0012"
#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=JetBrains.dotCover.CommandLineTools&version=2018.1.0"
#load "tasks/variables.cake";

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var artifacts = new DirectoryPath("./artifacts").MakeAbsolute(Context.Environment);

Task("Clean")
    .Does(() =>
{
    EnsureDirectoryExists(artifacts);
    CleanDirectory(artifacts);
});

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

Task("Restore (Unix)")
    .WithCriteria(IsRunningOnUnix)
    .Does(() =>
{
    MSBuild("./LSP.sln", settings => settings.SetConfiguration(configuration).WithTarget("Restore"));
});

Task("Restore (Windows)")
    .WithCriteria(IsRunningOnWindows)
    .Does(() =>
{
    DotNetCoreRestore();
});

Task("Restore")
.IsDependentOn("Restore (Unix)")
.IsDependentOn("Restore (Windows)");

Task("Build")
    .IsDependentOn("Restore")
    .DoesForEach(GetFiles("src/**/*.csproj").Concat(GetFiles("test/**/*.csproj")), (project) =>
    {
        MSBuild(project, settings =>
            settings
                .SetConfiguration(configuration)
                .WithTarget("Build"));
    });

Task("TestSetup")
    .Does(() => {
        CleanDirectory(artifacts + "/tests");
        CleanDirectory(artifacts + "/coverage");
        EnsureDirectoryExists(artifacts + "/tests");
        EnsureDirectoryExists(artifacts + "/coverage");
    });

Task("Test (Unix)")
    .WithCriteria(IsRunningOnUnix)
    .IsDependentOn("TestSetup")
    .IsDependentOn("Build")
    .DoesForEach(GetFiles("test/*/*.csproj"), (testProject) =>
{
    DotNetCoreTest(
        testProject.GetDirectory().FullPath,
        new DotNetCoreTestSettings() {
            NoBuild = true,
            Configuration = configuration,
            Framework = "netcoreapp2.1",
            EnvironmentVariables = GitVersionEnvironmentVariables,
            TestAdapterPath = ".",
            Logger = $"\"xunit;LogFilePath={string.Format("{0}/tests/{1}.xml", artifacts, testProject.GetFilenameWithoutExtension())}\"",
            ArgumentCustomization = args => args.Append("/p:CollectCoverage=true"),
        }
    );
});

Task("Test (Windows)")
    .WithCriteria(IsRunningOnWindows)
    .IsDependentOn("TestSetup")
    .IsDependentOn("Build")
    .DoesForEach(GetFiles("test/*/*.csproj"), (testProject) =>
{
    DotCoverCover(tool => {
        tool.DotNetCoreTest(
            testProject.GetDirectory().FullPath,
            new DotNetCoreTestSettings() {
                NoBuild = true,
                Configuration = configuration,
                Framework = "netcoreapp2.1",
                EnvironmentVariables = GitVersionEnvironmentVariables,
                TestAdapterPath = ".",
                Logger = $"\"xunit;LogFilePath={string.Format("{0}/tests/{1}.xml", artifacts, testProject.GetFilenameWithoutExtension())}\"",
                // ArgumentCustomization = args => args.Append("/p:CollectCoverage=true"),
            });
        },
        artifacts + "/coverage/coverage-"+ testProject.GetFilenameWithoutExtension() + ".dcvr",
        new DotCoverCoverSettings() {
                TargetWorkingDir = testProject.GetDirectory(),
                WorkingDirectory = testProject.GetDirectory(),
                EnvironmentVariables = GitVersionEnvironmentVariables,
            }
            .WithFilter("+:OmniSharp.*")
    );
})
.Finally(() => {
    DotCoverMerge(
        GetFiles(artifacts + "/coverage/*.dcvr"),
        artifacts + "/coverage/coverage.dcvr"
    );

    DotCoverReport(
        artifacts + "/coverage/coverage.dcvr",
        new FilePath(artifacts + "/coverage/coverage.html"),
        new DotCoverReportSettings {
            ReportType = DotCoverReportType.HTML
        }
    );

    DotCoverReport(
        artifacts + "/coverage/coverage.dcvr",
        new FilePath(artifacts + "/coverage/coverage.xml"),
        new DotCoverReportSettings {
            ReportType = DotCoverReportType.DetailedXML
        }
    );

    var withBom = System.IO.File.ReadAllText(artifacts + "/coverage/coverage.xml");
    System.IO.File.WriteAllText(artifacts + "/coverage/coverage.xml", withBom.Replace(_byteOrderMarkUtf8, ""));
});

Task("Test")
    .IsDependentOn("Test (Unix)")
    .IsDependentOn("Test (Windows)");

Task("Pack")
    .WithCriteria(IsRunningOnWindows) // TODO: Make work on travis
    .IsDependentOn("Build")
    .Does(() => EnsureDirectoryExists(artifacts + "/nuget"))
    .DoesForEach(GetFiles("src/*/*.csproj"), (project) => {
        DotNetCorePack(project.FullPath, new DotNetCorePackSettings
        {
            NoBuild = true,
            IncludeSymbols = true,
            Configuration = configuration,
            EnvironmentVariables = GitVersionEnvironmentVariables,
            OutputDirectory = artifacts + "/nuget"
        });
    });

Task("GitVersion")
    .Does(() => {
        GitVersion(new GitVersionSettings() {
            OutputType = GitVersionOutput.BuildServer
        });
    });

Task("Default")
    .IsDependentOn("Submodules")
    .IsDependentOn("Embed MediatR")
    .IsDependentOn("GitVersion")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack");

RunTarget(target);

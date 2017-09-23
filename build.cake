#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"
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

Task("Restore")
    .Does(() =>
{
    MSBuild("./LSP.sln", settings =>
        settings
            .SetConfiguration(configuration)
            .WithTarget("Restore"));
    // DotNetCoreRestore();
});

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

Task("Test (No Coverage)")
    .WithCriteria(IsRunningOnUnix)
    .IsDependentOn("TestSetup")
    .IsDependentOn("Build")
    .DoesForEach(GetFiles("test/*/*.csproj"), (testProject) =>
{
    DotNetCoreTool(
        testProject.GetDirectory().FullPath,
        "xunit",
        new ProcessArgumentBuilder()
            .AppendSwitchQuoted("-xml", string.Format("{0}/tests/{1}.xml", artifacts, testProject.GetFilenameWithoutExtension()))
            .AppendSwitch("-configuration", configuration)
            .Append("-noshadow"),
        new DotNetCoreToolSettings() {
            EnvironmentVariables = GitVersionEnvironmentVariables,
    });
});

Task("Test (Coverage)")
    .WithCriteria(IsRunningOnWindows)
    .IsDependentOn("TestSetup")
    .IsDependentOn("Build")
    .DoesForEach(GetFiles("test/*/*.csproj"), (testProject) =>
{
    DotCoverCover(tool => {
        tool.DotNetCoreTool(
            testProject.GetDirectory().FullPath,
            "xunit",
            new ProcessArgumentBuilder()
                .AppendSwitchQuoted("-xml", string.Format("{0}/tests/{1}.xml", artifacts, testProject.GetFilenameWithoutExtension()))
                .AppendSwitch("-configuration", configuration)
                .Append("-noshadow"),
            new DotNetCoreToolSettings() {
                EnvironmentVariables = GitVersionEnvironmentVariables,
            });
        },
        artifacts + "/coverage/coverage-"+ testProject.GetFilenameWithoutExtension() + ".dcvr",
        new DotCoverCoverSettings() {
                TargetWorkingDir = testProject.GetDirectory(),
                WorkingDirectory = testProject.GetDirectory(),
                EnvironmentVariables = GitVersionEnvironmentVariables,
            }
            .WithFilter("+:JsonRpc")
            .WithFilter("+:Lsp")
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
    .IsDependentOn("Test (Coverage)")
    .IsDependentOn("Test (No Coverage)");

Task("Pack")
    .IsDependentOn("Build")
    .Does(() => EnsureDirectoryExists(artifacts + "/nuget"))
    .DoesForEach(GetFiles("src/*/*.csproj"), (project) => {
        DotNetCorePack(project.FullPath, new DotNetCorePackSettings
        {
            NoBuild = true,
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
    .IsDependentOn("GitVersion")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack");

RunTarget(target);

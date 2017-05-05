#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"
#tool "nuget:?package=coveralls.io.dotcover"
#addin "Cake.Coveralls";

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
    DotNetCoreRestore();
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    foreach (var project in GetFiles("src/*/*.csproj").Concat(GetFiles("test/*/*.csproj")))
        DotNetCoreBuild(project.FullPath, new DotNetCoreBuildSettings
        {
            Configuration = configuration
        });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    EnsureDirectoryExists(artifacts + "/tests");
    EnsureDirectoryExists(artifacts + "/coverage");

    foreach (var testProject in GetFiles("test/*/*.csproj")) {
        StartProcess("dotnet", new ProcessSettings() {
            WorkingDirectory = testProject.GetDirectory(),
            Arguments = new ProcessArgumentBuilder()
                .Append("xunit")
                .Append("-noshadow")
                .AppendSwitch("-configuration", configuration)
                .AppendSwitchQuotedSecret("-xml", string.Format("{0}/tests/{1}.xml", artifacts, testProject.GetFilenameWithoutExtension()))
                .AppendSwitchQuotedSecret("-html", string.Format("{0}/tests/{1}.html", artifacts, testProject.GetFilenameWithoutExtension()))
        });
    }
});

Task("Coverage")
    //.IsDependentOn("Build")
    .Does(() =>
{
    CleanDirectory(artifacts + "/coverage");
    EnsureDirectoryExists(artifacts + "/coverage");

    foreach (var testProject in GetFiles("test/*/*.csproj")) {
        DotCoverCover(tool => {
            // tool.XUnit2()
                // tool.StartProcess(Context.Tools.Resolve("dotnet.exe"), new ProcessSettings() {
                //     WorkingDirectory = testProject.GetDirectory(),
                //     Arguments = new ProcessArgumentBuilder()
                //         .Append("test")
                //         .AppendSwitch("-c", configuration)
                //         .Append("--no-build")
                //         .Append("-f net46")
                // });
                tool.StartProcess(Context.Tools.Resolve("dotnet.exe"), new ProcessSettings() {
                    WorkingDirectory = testProject.GetDirectory(),
                    Arguments = new ProcessArgumentBuilder()
                        .Append("xunit")
                        .Append("-noshadow")
                        .AppendSwitch("-configuration", configuration)
                        .AppendSwitch("-framework", "net46")
                        .AppendSwitchQuotedSecret("-xml", string.Format("{0}/tests/{1}.xml", artifacts, testProject.GetFilenameWithoutExtension()))
                        .AppendSwitchQuotedSecret("-html", string.Format("{0}/tests/{1}.html", artifacts, testProject.GetFilenameWithoutExtension()))
                });
            },
            artifacts + "/coverage/coverage-"+ testProject.GetFilenameWithoutExtension() + ".dcvr",
            new DotCoverCoverSettings() {
                    // Register = "user",
                    // MergeOutput = true,
                    // OldStyle = true,
                    TargetWorkingDir = testProject.GetDirectory(),
                    WorkingDirectory = testProject.GetDirectory(),
                    // ReportType = DotCoverReportType.XML
                }
                .WithFilter("+:JsonRpc")
                .WithFilter("+:Lsp")
        );
    }

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
});

Task("Coveralls [AppVeyor]")
    .IsDependentOn("Coverage")
    .WithCriteria(AppVeyor.IsRunningOnAppVeyor)
    .Does(() => {
        CoverallsNet(artifacts + "/coverage/coverage.opencover", CoverallsNetReportType.OpenCover, new CoverallsNetSettings()
        {
            RepoToken = EnvironmentVariable("coveralls_repo_token"),
            UseRelativePaths = true,
            ServiceName = "Appveyor",
            CommitId = EnvironmentVariable("APPVEYOR_REPO_COMMIT"),
            CommitBranch = EnvironmentVariable("APPVEYOR_REPO_BRANCH"),
            CommitAuthor = EnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR"),
            CommitEmail = EnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL"),
            CommitMessage = EnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE") + (EnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE_EXTENDED") ?? string.Empty),
        });
    });

Task("Coveralls [TravisCI]")
    .IsDependentOn("Coverage")
    .WithCriteria(TravisCI.IsRunningOnTravisCI)
    .Does(() => {
        CoverallsNet(artifacts + "/coverage/coverage.opencover", CoverallsNetReportType.OpenCover, new CoverallsNetSettings()
        {
            RepoToken = EnvironmentVariable("coveralls_repo_token"),
            UseRelativePaths = true,
            ServiceName = "TravisCI",
            // CommitId = EnvironmentVariable("APPVEYOR_REPO_COMMIT"),
            // CommitBranch = EnvironmentVariable("APPVEYOR_REPO_BRANCH"),
            // CommitAuthor = EnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR"),
            // CommitEmail = EnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL"),
            // CommitMessage = EnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE") + (EnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE_EXTENDED") ?? string.Empty),
        });
    });

Task("Coveralls")
    .IsDependentOn("Coverage")
    .IsDependentOn("Coveralls [TravisCI]")
    .IsDependentOn("Coveralls [AppVeyor]");


Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Coveralls");

RunTarget(target);

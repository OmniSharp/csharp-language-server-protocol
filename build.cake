#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=OpenCover"
#tool "nuget:?package=coveralls.net"
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
        DotNetCoreBuild(project.FullPath);
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
                .AppendSwitchQuotedSecret("-xml", string.Format("{0}/tests/{1}.xml", artifacts, testProject.GetFilenameWithoutExtension()))
                .AppendSwitchQuotedSecret("-html", string.Format("{0}/tests/{1}.html", artifacts, testProject.GetFilenameWithoutExtension()))
        });
    }
});

Task("Coverage")
    .IsDependentOn("Build")
    .Does(() =>
{
    EnsureDirectoryExists(artifacts + "/coverage");

    foreach (var testProject in GetFiles("test/*/*.csproj")) {
        OpenCover(tool => {
                tool.StartProcess(Context.Tools.Resolve("dotnet.exe"), new ProcessSettings() {
                    WorkingDirectory = testProject.GetDirectory(),
                    Arguments = new ProcessArgumentBuilder()
                        .Append("test")
                        .Append("--no-build")
                        .Append("-f net46")

                });
            },
            artifacts + "/coverage/coverage.opencover",
            new OpenCoverSettings() {
                    Register = "user",
                    MergeOutput = true,
                    OldStyle = true,
                    WorkingDirectory = testProject.GetDirectory(),
                }
                .WithFilter("+[JsonRpc*]*")
                .WithFilter("+[Lsp*]*")
                .WithFilter("-[*.Tests]*")
        );
    }
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

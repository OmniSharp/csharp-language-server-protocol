using System.Collections.Generic;
using System.Linq;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Rocket.Surgery.Nuke.ContinuousIntegration;
using Rocket.Surgery.Nuke.DotNetCore;
using Rocket.Surgery.Nuke.GithubActions;


internal class LocalConstants
{
    public static string[] PathsIgnore =
    {
        ".codecov.yml",
        ".editorconfig",
        ".gitattributes",
        ".gitignore",
        ".gitmodules",
        ".lintstagedrc.js",
        ".prettierignore",
        ".prettierrc",
        "LICENSE",
        "nukeeper.settings.json",
        "omnisharp.json",
        "package-lock.json",
        "package.json",
        "Readme.md",
        ".github/dependabot.yml",
        ".github/labels.yml",
        ".github/release.yml",
        ".github/renovate.json",
    };
}

[GitHubActionsSteps(
    "ci-ignore",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    On = new[] { GitHubActionsTrigger.Push },
    OnPushTags = new[] { "v*" },
    OnPushBranches = new[] { "master", "main", "next" },
    OnPullRequestBranches = new[] { "master", "main", "next" },
    Enhancements = new[] { nameof(CiIgnoreMiddleware) }
)]
[GitHubActionsSteps(
    "ci",
    GitHubActionsImage.MacOsLatest,
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    On = new[] { GitHubActionsTrigger.Push },
    OnPushTags = new[] { "v*" },
    OnPushBranches = new[] { "master", "main", "next" },
    OnPullRequestBranches = new[] { "master", "main", "next" },
    InvokedTargets = new[] { nameof(Default) },
    NonEntryTargets = new[]
    {
        nameof(ICIEnvironment.CIEnvironment),
        nameof(ITriggerCodeCoverageReports.TriggerCodeCoverageReports),
        nameof(ITriggerCodeCoverageReports.GenerateCodeCoverageReportCobertura),
        nameof(IGenerateCodeCoverageBadges.GenerateCodeCoverageBadges),
        nameof(IGenerateCodeCoverageReport.GenerateCodeCoverageReport),
        nameof(IGenerateCodeCoverageSummary.GenerateCodeCoverageSummary),
        nameof(Default)
    },
    ExcludedTargets = new[] { nameof(ICanClean.Clean), nameof(ICanRestoreWithDotNetCore.DotnetToolRestore) },
    Enhancements = new[] { nameof(CiMiddleware) }
)]
[PrintBuildVersion]
[PrintCIEnvironment]
[UploadLogs]
[TitleEvents]
public partial class Solution
{
    public static RocketSurgeonGitHubActionsConfiguration CiIgnoreMiddleware(
        RocketSurgeonGitHubActionsConfiguration configuration
    )
    {
        foreach (var item in configuration.DetailedTriggers.OfType<RocketSurgeonGitHubActionsVcsTrigger>())
        {
            item.IncludePaths = LocalConstants.PathsIgnore;
        }

        configuration.Jobs.RemoveAt(1);
        ( (RocketSurgeonsGithubActionsJob)configuration.Jobs[0] ).Steps = new List<GitHubActionsStep>
        {
            new RunStep("N/A")
            {
                Run = "echo \"No build required\""
            }
        };

        return configuration;
    }

    public static RocketSurgeonGitHubActionsConfiguration CiMiddleware(
        RocketSurgeonGitHubActionsConfiguration configuration
    )
    {
        foreach (var item in configuration.DetailedTriggers.OfType<RocketSurgeonGitHubActionsVcsTrigger>())
        {
            item.ExcludePaths = LocalConstants.PathsIgnore;
        }

        var buildJob = configuration.Jobs.OfType<RocketSurgeonsGithubActionsJob>().First(z => z.Name == "Build");
        buildJob.FailFast = false;
        var checkoutStep = buildJob.Steps.OfType<CheckoutStep>().Single();
        // For fetch all
        checkoutStep.FetchDepth = 0;
        buildJob.Environment["NUGET_PACKAGES"] = "${{ github.workspace }}/.nuget/packages";
        buildJob.Steps.InsertRange(
            buildJob.Steps.IndexOf(checkoutStep) + 1,
            new BaseGitHubActionsStep[]
            {
                new RunStep("Fetch all history for all tags and branches")
                {
                    Run = "git fetch --prune"
                },
                new UsingStep("NuGet Cache")
                {
                    Uses = "actions/cache@v2",
                    With =
                    {
                        ["path"] = "${{ github.workspace }}/.nuget/packages",
                        // keep in mind using central package versioning here
                        ["key"] =
                            "${{ runner.os }}-nuget-${{ hashFiles('**/Directory.Packages.props') }}-${{ hashFiles('**/Directory.Packages.support.props') }}",
                        ["restore-keys"] = @"|
              ${{ runner.os }}-nuget-"
                    }
                },
                new SetupDotNetStep("Use .NET Core 3.1 SDK")
                {
                    DotNetVersion = "3.1.x"
                },
                new SetupDotNetStep("Use .NET Core 6.0 SDK")
                {
                    DotNetVersion = "6.0.x"
                },
            }
        );

        buildJob.Steps.Add(
            new UsingStep("Publish Coverage")
            {
                Uses = "codecov/codecov-action@v1",
                With = new Dictionary<string, string>
                {
                    ["name"] = "actions-${{ matrix.os }}",
                }
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish logs")
            {
                Name = "logs",
                Path = "artifacts/logs/",
                If = "always()"
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish coverage data")
            {
                Name = "coverage",
                Path = "coverage/",
                If = "always()"
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish test data")
            {
                Name = "test data",
                Path = "artifacts/test/",
                If = "always()"
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish NuGet Packages")
            {
                Name = "nuget",
                Path = "artifacts/nuget/",
                If = "always()"
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish Docs")
            {
                Name = "docs",
                Path = "artifacts/docs/",
                If = "always()"
            }
        );

        return configuration;
    }
}

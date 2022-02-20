using System.Collections.Generic;
using System.Linq;
using Nuke.Common.CI.GitHubActions;
using Rocket.Surgery.Nuke;
using Rocket.Surgery.Nuke.ContinuousIntegration;
using Rocket.Surgery.Nuke.DotNetCore;
using Rocket.Surgery.Nuke.GithubActions;

[AzurePipelinesSteps(
    AutoGenerate = false,
    InvokeTargets = new[] { nameof(Default) },
    NonEntryTargets = new[] {
        nameof(ICIEnvironment.CIEnvironment),
        nameof(ITriggerCodeCoverageReports.TriggerCodeCoverageReports),
        nameof(ITriggerCodeCoverageReports.GenerateCodeCoverageReportCobertura),
        nameof(IGenerateCodeCoverageBadges.GenerateCodeCoverageBadges),
        nameof(IGenerateCodeCoverageReport.GenerateCodeCoverageReport),
        nameof(IGenerateCodeCoverageSummary.GenerateCodeCoverageSummary),
        nameof(Default)
    },
    ExcludedTargets = new[]
        { nameof(ICanClean.Clean), nameof(ICanRestoreWithDotNetCore.Restore), nameof(ICanRestoreWithDotNetCore.DotnetToolRestore) },
    Parameters = new[] {
        nameof(IHaveCodeCoverage.CoverageDirectory), nameof(IHaveOutputArtifacts.ArtifactsDirectory), nameof(Verbosity),
        nameof(IHaveConfiguration.Configuration)
    }
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
    Enhancements = new[] { nameof(Middleware) }
)]
[PrintBuildVersion]
[PrintCIEnvironment]
[UploadLogs]
[TitleEvents]
public partial class Solution
{
    public static RocketSurgeonGitHubActionsConfiguration Middleware(RocketSurgeonGitHubActionsConfiguration configuration)
    {
        var buildJob = configuration.Jobs.OfType<RocketSurgeonsGithubActionsJob>().First(z => z.Name == "Build");
        buildJob.FailFast = false;
        var checkoutStep = buildJob.Steps.OfType<CheckoutStep>().Single();
        // For fetch all
        checkoutStep.FetchDepth = 0;
        buildJob.Environment["NUGET_PACKAGES"] = "${{ github.workspace }}/.nuget/packages";
        buildJob.Steps.InsertRange(
            buildJob.Steps.IndexOf(checkoutStep) + 1, new BaseGitHubActionsStep[] {
                new RunStep("Fetch all history for all tags and branches") {
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
                            "${{ runner.os }}-nuget-${{ hashFiles('**/Directory.Build.targets', '**/Directory.Build.props', '**/*.csproj') }}",
                        ["restore-keys"] = @"|
              ${{ runner.os }}-nuget-"
                    }
                },
                new SetupDotNetStep("Use .NET Core 3.1 SDK") {
                    DotNetVersion = "3.1.x"
                },
                new SetupDotNetStep("Use .NET Core 6.0 SDK") {
                    DotNetVersion = "6.0.x"
                },
            }
        );

        buildJob.Steps.Add(
            new UsingStep("Publish Coverage") {
                Uses = "codecov/codecov-action@v1",
                With = new Dictionary<string, string> {
                    ["name"] = "actions-${{ matrix.os }}",
                    ["fail_ci_if_error"] = "true",
                }
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish logs") {
                Name = "logs",
                Path = "artifacts/logs/",
                If = "always()"
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish coverage data") {
                Name = "coverage",
                Path = "coverage/",
                If = "always()"
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish test data") {
                Name = "test data",
                Path = "artifacts/test/",
                If = "always()"
            }
        );

        buildJob.Steps.Add(
            new UploadArtifactStep("Publish NuGet Packages") {
                Name = "nuget",
                Path = "artifacts/nuget/",
                If = "always()"
            }
        );

        return configuration;
    }
}

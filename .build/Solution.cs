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
    NonEntryTargets = new[]
    {
        nameof(ICIEnvironment.CIEnvironment),
        nameof(ITriggerCodeCoverageReports.Trigger_Code_Coverage_Reports),
        nameof(ITriggerCodeCoverageReports.Generate_Code_Coverage_Report_Cobertura),
        nameof(IGenerateCodeCoverageBadges.Generate_Code_Coverage_Badges),
        nameof(IGenerateCodeCoverageReport.Generate_Code_Coverage_Report),
        nameof(IGenerateCodeCoverageSummary.Generate_Code_Coverage_Summary),
        nameof(Default)
    },
    ExcludedTargets = new[]
        { nameof(ICanClean.Clean), nameof(ICanRestoreWithDotNetCore.Restore), nameof(ICanRestoreWithDotNetCore.DotnetToolRestore) },
    Parameters = new[]
    {
        nameof(IHaveCodeCoverage.CoverageDirectory), nameof(IHaveOutputArtifacts.ArtifactsDirectory), nameof(Verbosity),
        nameof(IHaveConfiguration.Configuration)
    }
)]
[GitHubActionsSteps("ci", GitHubActionsImage.MacOsLatest, GitHubActionsImage.WindowsLatest, GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    On = new[] { GitHubActionsTrigger.Push },
    OnPushTags = new[] { "v*" },
    OnPushBranches = new[] { "master", "next" },
    OnPullRequestBranches = new[] { "master", "next" },
    InvokedTargets = new[] { nameof(Default) },
    NonEntryTargets = new[]
    {
        nameof(ICIEnvironment.CIEnvironment),
        nameof(ITriggerCodeCoverageReports.Trigger_Code_Coverage_Reports),
        nameof(ITriggerCodeCoverageReports.Generate_Code_Coverage_Report_Cobertura),
        nameof(IGenerateCodeCoverageBadges.Generate_Code_Coverage_Badges),
        nameof(IGenerateCodeCoverageReport.Generate_Code_Coverage_Report),
        nameof(IGenerateCodeCoverageSummary.Generate_Code_Coverage_Summary),
        nameof(Default)
    },
    ExcludedTargets = new[] { nameof(ICanClean.Clean), nameof(ICanRestoreWithDotNetCore.DotnetToolRestore) },
    Enhancements = new[] { nameof(Middleware) }
)]
[PrintBuildVersion, PrintCIEnvironment, UploadLogs]
public partial class Solution
{
    public static RocketSurgeonGitHubActionsConfiguration Middleware(RocketSurgeonGitHubActionsConfiguration configuration)
    {
        var buildJob = configuration.Jobs.First(z => z.Name == "Build");
        var checkoutStep = buildJob.Steps.OfType<CheckoutStep>().Single();
        // For fetch all
        checkoutStep.FetchDepth = 0;
        buildJob.Steps.InsertRange(buildJob.Steps.IndexOf(checkoutStep) + 1, new BaseGitHubActionsStep[] {
            new RunStep("Fetch all history for all tags and branches") {
                Run = "git fetch --prune"
            },
            new SetupDotNetStep("Use .NET Core 2.1 SDK") {
                DotNetVersion = "2.1.x"
            },
            new SetupDotNetStep("Use .NET Core 3.1 SDK") {
                DotNetVersion = "3.1.x"
            },
            new RunStep("🪓 **DOTNET HACK** 🪓") {
                Shell = GithubActionShell.Pwsh,
                Run = @"$version = Split-Path (Split-Path $ENV:DOTNET_ROOT -Parent) -Leaf;
                        $root = Split-Path (Split-Path $ENV:DOTNET_ROOT -Parent) -Parent;
                        $directories = Get-ChildItem $root | Where-Object { $_.Name -ne $version };
                        foreach ($dir in $directories) {
                            $from = $dir.FullName;
                            $to = ""$root/$version"";
                            Write-Host Copying from $from to $to;
                            Copy-Item ""$from\*"" $to -Recurse -Force;
                        }
                        "
            },
        });

        buildJob.Steps.Add(new UsingStep("Publish Coverage")
        {
            Uses = "codecov/codecov-action@v1",
            With = new Dictionary<string, string>
            {
                ["name"] = "actions-${{ matrix.os }}",
                ["fail_ci_if_error"] = "true",
            }
        });

        buildJob.Steps.Add(new UploadArtifactStep("Publish logs")
        {
            Name = "logs",
            Path = "artifacts/logs/",
            If = "always()"
        });

        buildJob.Steps.Add(new UploadArtifactStep("Publish coverage data")
        {
            Name = "coverage",
            Path = "coverage/",
            If = "always()"
        });

        buildJob.Steps.Add(new UploadArtifactStep("Publish test data")
        {
            Name = "test data",
            Path = "artifacts/test/",
            If = "always()"
        });

        buildJob.Steps.Add(new UploadArtifactStep("Publish NuGet Packages")
        {
            Name = "nuget",
            Path = "artifacts/nuget/",
            If = "always()"
        });


        /*

  - publish: "${{ parameters.Artifacts }}/logs/"
    displayName: Publish Logs
    artifact: "Logs${{ parameters.Postfix }}"
    condition: always()

  - publish: ${{ parameters.Coverage }}
    displayName: Publish Coverage
    artifact: "Coverage${{ parameters.Postfix }}"
    condition: always()

  - publish: "${{ parameters.Artifacts }}/nuget/"
    displayName: Publish NuGet Artifacts
    artifact: "NuGet${{ parameters.Postfix }}"
    condition: always()
        */
        return configuration;
    }
}

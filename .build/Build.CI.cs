using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Rocket.Surgery.Nuke.ContinuousIntegration;
using Rocket.Surgery.Nuke.DotNetCore;
using Rocket.Surgery.Nuke.GithubActions;
using Rocket.Surgery.Nuke.Jobs;

#pragma warning disable CA1050

[GitHubActionsSteps(
    "ci-ignore",
    GitHubActionsImage.MacOsLatest,
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    On = [RocketSurgeonGitHubActionsTrigger.Push],
    OnPushTags = ["v*"],
    OnPushBranches = ["master", "main", "next"],
    OnPullRequestBranches = ["master", "main", "next"],
    Enhancements = [nameof(CiIgnoreMiddleware)]
)]
[GitHubActionsSteps(
    "ci",
    GitHubActionsImage.MacOsLatest,
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    On = [RocketSurgeonGitHubActionsTrigger.Push],
    OnPushTags = ["v*"],
    OnPushBranches = ["master", "main", "next"],
    OnPullRequestBranches = ["master", "main", "next"],
    InvokedTargets = [nameof(Default)],
    NonEntryTargets = [
        nameof(ICIEnvironment.CIEnvironment),
        nameof(ITriggerCodeCoverageReports.GenerateCodeCoverageReportCobertura),
        nameof(IGenerateCodeCoverageBadges.GenerateCodeCoverageBadges),
        nameof(IGenerateCodeCoverageReport.GenerateCodeCoverageReport),
        nameof(IGenerateCodeCoverageSummary.GenerateCodeCoverageSummary),
        nameof(Default),
    ],
    ExcludedTargets = [nameof(ICanClean.Clean), nameof(ICanRestoreWithDotNetCore.DotnetToolRestore)],
    Enhancements = [nameof(CiMiddleware)]
)]
[CloseMilestoneJob(AutoGenerate = false)]
[DraftReleaseJob(AutoGenerate = false)]
[UpdateMilestoneJob(AutoGenerate = false)]
[PrintBuildVersion]
[PrintCIEnvironment]
[UploadLogs]
[TitleEvents]
[ContinuousIntegrationConventions]
internal sealed partial class Pipeline
{
    public static RocketSurgeonGitHubActionsConfiguration CiIgnoreMiddleware(
        RocketSurgeonGitHubActionsConfiguration configuration
    )
    {
        ( (RocketSurgeonsGithubActionsJob)configuration.Jobs[0] ).Steps = new List<GitHubActionsStep>
        {
            new RunStep("N/A")
            {
                Run = "echo \"No build required\""
            }
        };

        return configuration.IncludeRepositoryConfigurationFiles();
    }

    public static RocketSurgeonGitHubActionsConfiguration CiMiddleware(
        RocketSurgeonGitHubActionsConfiguration configuration
    )
    {
        configuration
            .ExcludeRepositoryConfigurationFiles()
            .Jobs.OfType<RocketSurgeonsGithubActionsJob>()
            .First(z => z.Name.Equals("Build", StringComparison.OrdinalIgnoreCase))
            .ConfigureStep<CheckoutStep>(step => step.FetchDepth = 0)
            .UseDotNetSdks("6.0", "8.0", "9.0")
            .AddNuGetCache()
            .AddVscodeExtensionTests()
            .PublishLogs<Pipeline>()
            .PublishArtifacts<Pipeline>()
            .FailFast = false;

        return configuration;
    }
}

public static class JobExtensions
{
    public static RocketSurgeonsGithubActionsJob AddVscodeExtensionTests(this RocketSurgeonsGithubActionsJob job)
    {
        return job
            .AddStep(new RunStep("Npm install") {
                Run = "npm ci",
                WorkingDirectory = Pipeline.VscodeTestExtensionProjectDirectory
            })
            .AddStep(new HeadlessRunStep("Vscode extension tests") {
                Run = "npm run test",
                WorkingDirectory = Pipeline.VscodeTestExtensionProjectDirectory
            });
    }
}

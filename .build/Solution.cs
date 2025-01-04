using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Rocket.Surgery.Nuke.ContinuousIntegration;
using Rocket.Surgery.Nuke.DotNetCore;
using Rocket.Surgery.Nuke.GithubActions;

namespace Build;

internal class LocalConstants
{
    public static string[] PathsIgnore => new[]
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
    On = new[] { RocketSurgeonGitHubActionsTrigger.Push },
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
    On = new[] { RocketSurgeonGitHubActionsTrigger.Push },
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
[ContinuousIntegrationConventions]
public partial class Solution
{
    public static RocketSurgeonGitHubActionsConfiguration CiIgnoreMiddleware(
        RocketSurgeonGitHubActionsConfiguration configuration
    )
    {
        configuration.IncludeRepositoryConfigurationFiles();

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
        configuration
            .ExcludeRepositoryConfigurationFiles()
            .AddNugetPublish()
            .Jobs.OfType<RocketSurgeonsGithubActionsJob>()
            .First(z => z.Name.Equals("Build", StringComparison.OrdinalIgnoreCase))
            .ConfigureStep<CheckoutStep>(step => step.FetchDepth = 0)
            .UseDotNetSdks("3.1", "6.0", "8.0", "9.0")
            .AddNuGetCache()
            .AddVscodeExtensionTests()
            .PublishLogs<Solution>()
            .PublishArtifacts<Solution>()
            .FailFast = false;

        return configuration;
    }
}

public static class Extensions
{
    public static RocketSurgeonsGithubActionsJob AddVscodeExtensionTests(this RocketSurgeonsGithubActionsJob job)
    {
        return job
            .AddStep(new RunStep("Npm install") {
                Run = "npm ci",
                WorkingDirectory = Solution.VscodeTestExtensionProjectDirectory
            })
            .AddStep(new HeadlessRunStep("Vscode extension tests") {
                Run = "npm run test",
                WorkingDirectory = Solution.VscodeTestExtensionProjectDirectory
            });
    }
}

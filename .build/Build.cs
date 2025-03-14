using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.Npm;
using Rocket.Surgery.Nuke.DotNetCore;

#pragma warning disable CA1050

[PublicAPI]
[UnsetVisualStudioEnvironmentVariables]
[PackageIcon("http://www.omnisharp.net/images/logo.png")]
[DotNetVerbosityMapping]
[MSBuildVerbosityMapping]
[NuGetVerbosityMapping]
[ShutdownDotNetAfterServerBuild]
[LocalBuildConventions]
internal sealed partial class Pipeline : NukeBuild,
                                ICanRestoreWithDotNetCore,
                                ICanBuildWithDotNetCore,
                                ICanTestWithDotNetCore,
                                ICanPackWithDotNetCore,
                                ICanClean,
                                IHavePublicApis,
                                IGenerateCodeCoverageReport,
                                IGenerateCodeCoverageSummary,
                                IGenerateCodeCoverageBadges,
                                IHaveConfiguration<Configuration>
{
    /// <summary>
    /// Support plugins are available for:
    /// - JetBrains ReSharper        https://nuke.build/resharper
    /// - JetBrains Rider            https://nuke.build/rider
    /// - Microsoft VisualStudio     https://nuke.build/visualstudio
    /// - Microsoft VSCode           https://nuke.build/vscode
    /// </summary>
    public static int Main() => Execute<Pipeline>(x => x.Default);

    public Target Build => _ => _;

    public Target Clean => _ => _;

    [Parameter("Configuration to build")]
    public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [OptionalGitRepository]
    public GitRepository? GitRepository { get; }

    [GitVersion(NoFetch = true, NoCache = false)]
    public GitVersion GitVersion { get; } = null!;

    public Target Lint => _ => _;

    public Target Pack => _ => _.DependsOn(Clean);

    public Target Restore => _ => _;

    public Target Test => _ => _;

    [NonEntryTarget]
    private Target Default => _ => _
                                  .DependsOn(Restore)
                                  .DependsOn(Build)
                                  .DependsOn(Test)
                                  .DependsOn(Pack);

#pragma warning disable CA1822 
    public Target NpmInstall => _ => _
#pragma warning restore CA1822 // Member 'NpmInstall' does not access instance data and can be marked as static
        .Executes(() =>
            NpmTasks.NpmCi(s => s
                .SetProcessWorkingDirectory(VscodeTestExtensionProjectDirectory)));

    public Target TestVscodeExtension => _ => _
        .DependsOn(NpmInstall)
        .Executes(() =>
            NpmTasks.NpmRun(s => s
                .SetProcessWorkingDirectory(VscodeTestExtensionProjectDirectory)
                .SetCommand("test")));

    public Target BuildVersion => _ => _.Before(Default).Before(Clean);

    internal const string VscodeTestExtensionProjectDirectory = "vscode-testextension";
}

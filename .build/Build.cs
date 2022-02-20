using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Rocket.Surgery.Nuke;
using Rocket.Surgery.Nuke.DotNetCore;

[PublicAPI]
[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[PackageIcon("http://www.omnisharp.net/images/logo.png")]
[EnsureReadmeIsUpdated]
[DotNetVerbosityMapping]
[MSBuildVerbosityMapping]
[NuGetVerbosityMapping]
public partial class Solution : NukeBuild,
                                ICanRestoreWithDotNetCore,
                                ICanBuildWithDotNetCore,
                                ICanTestWithDotNetCore,
                                ICanPackWithDotNetCore,
                                IHaveDataCollector,
                                ICanClean,
                                ICanUpdateReadme,
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
    public static int Main() => Execute<Solution>(x => x.Default);

    [OptionalGitRepository] public GitRepository? GitRepository { get; }

    private Target Default => _ => _
                                  .DependsOn(Restore)
                                  .DependsOn(Build)
                                  .DependsOn(Test)
                                  .DependsOn(Pack);

    public Target Build => _ => _.Inherit<ICanBuildWithDotNetCore>(x => x.CoreBuild);

    public Target Pack => _ => _.Inherit<ICanPackWithDotNetCore>(x => x.CorePack)
                                .DependsOn(Clean);

    [ComputedGitVersion] public GitVersion GitVersion { get; } = null!;

    public Target Clean => _ => _.Inherit<ICanClean>(x => x.Clean);
    public Target Restore => _ => _.Inherit<ICanRestoreWithDotNetCore>(x => x.CoreRestore);

    public Target Test => _ => _.Inherit<ICanTestWithDotNetCore>(x => x.CoreTest);

    public Target BuildVersion => _ => _.Inherit<IHaveBuildVersion>(x => x.BuildVersion)
                                        .Before(Default)
                                        .Before(Clean);

    [Parameter("Configuration to build")] public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    AbsolutePath ICanUpdateReadme.ReadmeFilePath => RootDirectory / "README.md";
}

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.Npm;
using Rocket.Surgery.Nuke.DotNetCore;

namespace Build;

[PublicAPI]
[UnsetVisualStudioEnvironmentVariables]
[PackageIcon("http://www.omnisharp.net/images/logo.png")]
[DotNetVerbosityMapping]
[MSBuildVerbosityMapping]
[NuGetVerbosityMapping]
[LocalBuildConventions]
public sealed partial class Solution : NukeBuild,
                                ICanRestoreWithDotNetCore,
                                ICanBuildWithDotNetCore,
                                ICanTestWithDotNetCore,
                                ICanPackWithDotNetCore,
                                ICanClean,
                                ICanUpdateReadme,
                                IGenerateCodeCoverageReport,
                                IGenerateCodeCoverageSummary,
                                IGenerateCodeCoverageBadges,
                                IGenerateDocFx,
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

    public Target Build => _ => _.Inherit<ICanBuildWithDotNetCore>(x => x.DotnetCoreBuild);

    public Target Pack => _ => _.Inherit<ICanPackWithDotNetCore>(x => x.DotnetCorePack)
                                .DependsOn(Clean);

    [ComputedGitVersion] public GitVersion GitVersion { get; } = null!;

    public Target Clean => _ => _.Inherit<ICanClean>(x => x.CleanWellKnownTemporaryFiles);
    public Target Restore => _ => _.Inherit<ICanRestoreWithDotNetCore>(x => x.DotnetCoreRestore);

    public Target Test => _ => _.Inherit<ICanTestWithDotNetCore>(x => x.DotnetCoreTest);

    public Target NpmInstall => _ => _
        .Executes(() =>
            NpmTasks.NpmCi(s => s
                .SetProcessWorkingDirectory(VscodeTestExtensionProjectDirectory)));

    public Target TestVscodeExtension => _ => _
        .DependsOn(NpmInstall)
        .Executes(() =>
            NpmTasks.NpmRun(s => s
                .SetProcessWorkingDirectory(VscodeTestExtensionProjectDirectory)
                .SetCommand("test")));

    public Target BuildVersion => _ => _.Inherit<IHaveBuildVersion>(x => x.BuildVersion)
                                        .Before(Default)
                                        .Before(Clean);

    public Target Docs => _ => _.Inherit<IGenerateDocFx>(x => x.GenerateDocFx);

    [Parameter("Configuration to build")] public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    AbsolutePath ICanUpdateReadme.ReadmeFilePath => RootDirectory / "README.md";

    internal const string VscodeTestExtensionProjectDirectory = "vscode-testextension";
}

Lazy<GitVersion> GitVersionInstance = new Lazy<GitVersion>(() => {
    return GitVersion();
});

Dictionary<string, string> GitVersionEnvironmentVariables { get {
    var gv = GitVersionInstance.Value;

    return new Dictionary<string, string>() {
        { "GitVersion_Major", gv.Major.ToString() },
        { "GitVersion_Minor", gv.Minor.ToString() },
        { "GitVersion_Patch", gv.Patch.ToString() },
        { "GitVersion_PreReleaseTag", gv.PreReleaseTag },
        { "GitVersion_PreReleaseTagWithDash", gv.PreReleaseTagWithDash },
        { "GitVersion_PreReleaseLabel", gv.PreReleaseLabel },
        { "GitVersion_PreReleaseNumber", gv.PreReleaseNumber.ToString() },
        { "GitVersion_BuildMetaData", gv.BuildMetaData },
        { "GitVersion_BuildMetaDataPadded", gv.BuildMetaDataPadded },
        { "GitVersion_FullBuildMetaData", gv.FullBuildMetaData },
        { "GitVersion_MajorMinorPatch", gv.MajorMinorPatch },
        { "GitVersion_SemVer", gv.SemVer },
        { "GitVersion_LegacySemVer", gv.LegacySemVer },
        { "GitVersion_LegacySemVerPadded", gv.LegacySemVerPadded },
        { "GitVersion_AssemblySemVer", gv.AssemblySemVer },
        { "GitVersion_FullSemVer", gv.FullSemVer },
        { "GitVersion_InformationalVersion", gv.InformationalVersion },
        { "GitVersion_BranchName", gv.BranchName },
        { "GitVersion_Sha", gv.Sha },
        { "GitVersion_NuGetVersion", gv.NuGetVersion },
        { "GitVersion_CommitsSinceVersionSource", gv.CommitsSinceVersionSource.ToString() },
        { "GitVersion_CommitsSinceVersionSourcePadded", gv.CommitsSinceVersionSourcePadded },
        { "GitVersion_CommitDate", gv.CommitDate },
    };
} }
var _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

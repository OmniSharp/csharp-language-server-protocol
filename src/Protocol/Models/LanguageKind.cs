using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models;

/// <summary>
/// Known language identifiers.
///
/// @since 3.18.0
/// </summary>
[StringEnum]
public readonly partial struct LanguageKind
{
    public static LanguageKind ABAP { get; } = new("abap");
    public static LanguageKind WindowsBat { get; } = new("bat");
    public static LanguageKind BibTeX { get; } = new("bibtex");
    public static LanguageKind Clojure { get; } = new("clojure");
    public static LanguageKind Coffeescript { get; } = new("coffeescript");
    public static LanguageKind C { get; } = new("c");
    public static LanguageKind CPP { get; } = new("cpp");
    public static LanguageKind CSharp { get; } = new("csharp");
    public static LanguageKind CSS { get; } = new("css");
    public static LanguageKind D { get; } = new("d");
    public static LanguageKind Delphi { get; } = new("pascal");
    public static LanguageKind Diff { get; } = new("diff");
    public static LanguageKind Dart { get; } = new("dart");
    public static LanguageKind Dockerfile { get; } = new("dockerfile");
    public static LanguageKind Elixir { get; } = new("elixir");
    public static LanguageKind Erlang { get; } = new("erlang");
    public static LanguageKind FSharp { get; } = new("fsharp");
    public static LanguageKind GitCommit { get; } = new("git-commit");
    public static LanguageKind GitRebase { get; } = new("git-rebase");
    public static LanguageKind Go { get; } = new("go");
    public static LanguageKind Groovy { get; } = new("groovy");
    public static LanguageKind Handlebars { get; } = new("handlebars");
    public static LanguageKind Haskell { get; } = new("haskell");
    public static LanguageKind HTML { get; } = new("html");
    public static LanguageKind Ini { get; } = new("ini");
    public static LanguageKind Java { get; } = new("java");
    public static LanguageKind JavaScript { get; } = new("javascript");
    public static LanguageKind JavaScriptReact { get; } = new("javascriptreact");
    public static LanguageKind JSON { get; } = new("json");
    public static LanguageKind LaTeX { get; } = new("latex");
    public static LanguageKind Less { get; } = new("less");
    public static LanguageKind Lua { get; } = new("lua");
    public static LanguageKind Makefile { get; } = new("makefile");
    public static LanguageKind Markdown { get; } = new("markdown");
    public static LanguageKind ObjectiveC { get; } = new("objective-c");
    public static LanguageKind ObjectiveCPP { get; } = new("objective-cpp");
    public static LanguageKind Pascal { get; } = new("pascal");
    public static LanguageKind Perl { get; } = new("perl");
    public static LanguageKind Perl6 { get; } = new("perl6");
    public static LanguageKind PHP { get; } = new("php");
    public static LanguageKind Plaintext { get; } = new("plaintext");
    public static LanguageKind Powershell { get; } = new("powershell");
    public static LanguageKind Pug { get; } = new("jade");
    public static LanguageKind Python { get; } = new("python");
    public static LanguageKind R { get; } = new("r");
    public static LanguageKind Razor { get; } = new("razor");
    public static LanguageKind Ruby { get; } = new("ruby");
    public static LanguageKind Rust { get; } = new("rust");
    public static LanguageKind SCSS { get; } = new("scss");
    public static LanguageKind SASS { get; } = new("sass");
    public static LanguageKind Scala { get; } = new("scala");
    public static LanguageKind ShaderLab { get; } = new("shaderlab");
    public static LanguageKind ShellScript { get; } = new("shellscript");
    public static LanguageKind SQL { get; } = new("sql");
    public static LanguageKind Swift { get; } = new("swift");
    public static LanguageKind TypeScript { get; } = new("typescript");
    public static LanguageKind TypeScriptReact { get; } = new("typescriptreact");
    public static LanguageKind TeX { get; } = new("tex");
    public static LanguageKind VisualBasic { get; } = new("vb");
    public static LanguageKind XML { get; } = new("xml");
    public static LanguageKind XSL { get; } = new("xsl");
    public static LanguageKind YAML { get; } = new("yaml");
}

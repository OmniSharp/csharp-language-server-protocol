using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Minimatch;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    public interface ITextDocumentIdentifier
    {
        /// <summary>
        /// Returns the attributes for the document at the given URI.  This can return null.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        IEnumerable<TextDocumentAttributes> GetTextDocumentAttributes(DocumentUri uri);
    }

    public abstract class TextDocumentIdentifierBase : ITextDocumentIdentifier
    {
        public IEnumerable<TextDocumentAttributes> GetTextDocumentAttributes(DocumentUri uri)
        {
            if (GetAttributes(uri, out var languageId, out var scheme))
            {
                yield return new TextDocumentAttributes(uri, languageId, scheme);
            }
        }

        protected abstract bool GetAttributes(DocumentUri uri, out string languageId, out string scheme);
    }

    public class WellKnownLanguageIdentifier : TextDocumentIdentifierBase
    {
        private readonly string _languageId;
        private readonly Minimatcher[] _minimatchers;
        public WellKnownLanguageIdentifier(
            string name,
            string languageId,
            params string[] filePatterns)
        {
            Name = name;
            _languageId = languageId;
            _minimatchers = filePatterns.Select(z => new Minimatcher(z)).ToArray();
        }

        public string Name { get; }

        protected override bool GetAttributes(DocumentUri uri, out string languageId, out string scheme)
        {
            languageId = _languageId;
            scheme = uri.Scheme;
            return _minimatchers.Any(z => z.IsMatch(uri.ToString()));
        }
    }

    public static class WellKnownLanguages
    {
        public static readonly WellKnownLanguageIdentifier ABAP = new WellKnownLanguageIdentifier("ABAP", "abap", "**/*.abap");
        public static readonly WellKnownLanguageIdentifier WindowsBatch  = new WellKnownLanguageIdentifier("Windows Bat", "bat", "**/*.{bat,cmd}");
        public static readonly WellKnownLanguageIdentifier BibTeX  = new WellKnownLanguageIdentifier("BibTeX", "bibtex", "**/*.bib");
        public static readonly WellKnownLanguageIdentifier Clojure  = new WellKnownLanguageIdentifier("Clojure", "clojure", "**/*.{clj,cljs,cljc,ed}");
        public static readonly WellKnownLanguageIdentifier CoffeeScript  = new WellKnownLanguageIdentifier("Coffeescript", "coffeescript", "**/*.{coffee,litcoffee}");
        public static readonly WellKnownLanguageIdentifier C  = new WellKnownLanguageIdentifier("C", "c", "**/*.{c,h}");
        public static readonly WellKnownLanguageIdentifier CPlusPlus  = new WellKnownLanguageIdentifier("C++", "cpp", "**/*.{c,cc,cpp,cxx,c++,h,hh,hpp,hxx,h++}");
        public static readonly WellKnownLanguageIdentifier CSharp  = new WellKnownLanguageIdentifier("C#", "csharp", "**/*.{cs,csi,csx,cake}");
        public static readonly WellKnownLanguageIdentifier CSS  = new WellKnownLanguageIdentifier("CSS", "css", "**/*.css");
        public static readonly WellKnownLanguageIdentifier Diff  = new WellKnownLanguageIdentifier("Diff", "diff", "**/*.diff");
        public static readonly WellKnownLanguageIdentifier Dart  = new WellKnownLanguageIdentifier("Dart", "dart", "**/*.dart");
        public static readonly WellKnownLanguageIdentifier Dockerfile  = new WellKnownLanguageIdentifier("Dockerfile", "dockerfile", "**/*.dockerfile");
        public static readonly WellKnownLanguageIdentifier Elixir  = new WellKnownLanguageIdentifier("Elixir", "elixir", "**/*.{ex,exs}");
        public static readonly WellKnownLanguageIdentifier Erlang  = new WellKnownLanguageIdentifier("Erlang", "erlang", "**/*.{erl,hrl}");
        public static readonly WellKnownLanguageIdentifier FSharp  = new WellKnownLanguageIdentifier("F#", "fsharp", "**/*.{fs,fsi,fsx,fsscript}");
        public static readonly WellKnownLanguageIdentifier Go  = new WellKnownLanguageIdentifier("Go", "go", "**/*.go");
        public static readonly WellKnownLanguageIdentifier Groovy  = new WellKnownLanguageIdentifier("Groovy", "groovy", "**/*.{groovy,gvy,gy,gsh}");
        public static readonly WellKnownLanguageIdentifier Handlebars  = new WellKnownLanguageIdentifier("Handlebars", "handlebars", "**/*.{handlebars,hbs,mustache}");
        public static readonly WellKnownLanguageIdentifier HTML  = new WellKnownLanguageIdentifier("HTML", "html", "**/*.{html,htm}");
        public static readonly WellKnownLanguageIdentifier Ini  = new WellKnownLanguageIdentifier("Ini", "ini", "**/*.ini");
        public static readonly WellKnownLanguageIdentifier Java  = new WellKnownLanguageIdentifier("Java", "java", "**/*.{java,class}");
        public static readonly WellKnownLanguageIdentifier JavaScript  = new WellKnownLanguageIdentifier("JavaScript", "javascript", "**/*.{js,mjs}");
        public static readonly WellKnownLanguageIdentifier JavaScriptReact  = new WellKnownLanguageIdentifier("JavaScript React", "javascriptreact", "**/*.jsx");
        public static readonly WellKnownLanguageIdentifier JSON  = new WellKnownLanguageIdentifier("JSON", "json", "**/*.json");
        public static readonly WellKnownLanguageIdentifier LaTeX  = new WellKnownLanguageIdentifier("TeX", "latex", "**/*.tex");
        public static readonly WellKnownLanguageIdentifier Less  = new WellKnownLanguageIdentifier("Less", "less", "**/*.less");
        public static readonly WellKnownLanguageIdentifier Lua  = new WellKnownLanguageIdentifier("Lua", "lua", "**/*.lua");
        public static readonly WellKnownLanguageIdentifier Markdown  = new WellKnownLanguageIdentifier("Markdown", "markdown", "**/*.{markdown,mdown,mkdn,mkd,md}");
        public static readonly WellKnownLanguageIdentifier ObjectiveC  = new WellKnownLanguageIdentifier("Objective-C", "objective-c", "**/*.{h,m,mm,M}");
        public static readonly WellKnownLanguageIdentifier ObjectiveCPlusPlus  = new WellKnownLanguageIdentifier("Objective-C++", "objective-cpp", "**/*.{h,mm}");
        public static readonly WellKnownLanguageIdentifier Perl  = new WellKnownLanguageIdentifier("Perl", "perl", "**/*.{plx,pl,pm,xs,t,pod}");
        public static readonly WellKnownLanguageIdentifier Raku  = new WellKnownLanguageIdentifier("Raku", "raku", "**/*.{p6,pm6,pod6,t6,raku,rakumod,rakudoc,rakutest}");
        public static readonly WellKnownLanguageIdentifier PHP  = new WellKnownLanguageIdentifier("PHP", "php", "**/*.{php,phtml,php3,php4,php5,php7,phps,php-s,pht,pha}");
        public static readonly WellKnownLanguageIdentifier PowerShell  = new WellKnownLanguageIdentifier("PowerShell", "powershell", "**/*.{ps1,psd1,psm1}");
        public static readonly WellKnownLanguageIdentifier Pug  = new WellKnownLanguageIdentifier("Pug", "jade", "**/*.{pug}");
        public static readonly WellKnownLanguageIdentifier Python  = new WellKnownLanguageIdentifier("Python", "python", "**/*.{py,pyi,pyc,pyd,pyo,pyw,pyz}");
        public static readonly WellKnownLanguageIdentifier R  = new WellKnownLanguageIdentifier("R", "r", "**/*.{r,rdata,rds,rda}");
        public static readonly WellKnownLanguageIdentifier Razor  = new WellKnownLanguageIdentifier("Razor", "razor", "**/*.{razor,cshtml}");
        public static readonly WellKnownLanguageIdentifier Ruby  = new WellKnownLanguageIdentifier("Ruby", "ruby", "**/*.rb");
        public static readonly WellKnownLanguageIdentifier Rust  = new WellKnownLanguageIdentifier("Rust", "rust", "**/*.{rs,rlib}");
        public static readonly WellKnownLanguageIdentifier SCSS  = new WellKnownLanguageIdentifier("SCSS", "scss", "**/*.{scss,sass}");
        public static readonly WellKnownLanguageIdentifier Scala  = new WellKnownLanguageIdentifier("Scala", "scala", "**/*.{scala,sc}");
        public static readonly WellKnownLanguageIdentifier SQL  = new WellKnownLanguageIdentifier("SQL", "sql", "**/*.sql");
        public static readonly WellKnownLanguageIdentifier Swift  = new WellKnownLanguageIdentifier("Swift", "swift", "**/*.swift");
        public static readonly WellKnownLanguageIdentifier TypeScript  = new WellKnownLanguageIdentifier("TypeScript", "typescript", "**/*.ts");
        public static readonly WellKnownLanguageIdentifier TypeScriptReact  = new WellKnownLanguageIdentifier("TypeScript React", "typescriptreact", "**/*.tsx");
        public static readonly WellKnownLanguageIdentifier TeX  = new WellKnownLanguageIdentifier("TeX", "tex", "**/*.tex");
        public static readonly WellKnownLanguageIdentifier VisualBasic  = new WellKnownLanguageIdentifier("Visual Basic", "vb", "**/*.vb");
        public static readonly WellKnownLanguageIdentifier XML  = new WellKnownLanguageIdentifier("XML", "xml", "**/*.xml");
        public static readonly WellKnownLanguageIdentifier XSL  = new WellKnownLanguageIdentifier("XSL", "xsl", "**/*.xsl");
        public static readonly WellKnownLanguageIdentifier YAML  = new WellKnownLanguageIdentifier("YAML", "yaml", "**/*.{yaml,yml}");
    }




    public class TextDocumentIdentifiers : IEnumerable<ITextDocumentIdentifier>
    {
        private readonly HashSet<ITextDocumentIdentifier> _textDocumentIdentifiers = new HashSet<ITextDocumentIdentifier>();
        public IEnumerator<ITextDocumentIdentifier> GetEnumerator() => _textDocumentIdentifiers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IDisposable Add(params ITextDocumentIdentifier[] identifiers)
        {
            foreach (var item in identifiers)
                _textDocumentIdentifiers.Add(item);
            return Disposable.Create(() => {
                foreach (var textDocumentIdentifier in identifiers)
                {
                    _textDocumentIdentifiers.Remove(textDocumentIdentifier);
                }
            });
        }
    }
}

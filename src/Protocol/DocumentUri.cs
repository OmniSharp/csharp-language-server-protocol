using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static System.IO.Path;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// This class describes a document uri as defined by https://microsoft.github.io/language-server-protocol/specifications/specification-current/#uri
    /// </summary>
    /// <remarks>This exists because of some non-standard serialization in vscode around uris and .NET's behavior when deserializing those uris</remarks>
    public class DocumentUri : IEquatable<DocumentUri>
    {
        private static readonly Regex WindowsPath =
            new Regex(@"^\w(?:\:|%3a)[\\|\/]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly string _delimiter = SchemeDelimiter;

        /// <summary>
        /// Create a new document uri
        /// </summary>
        /// <param name="url"></param>
        public DocumentUri(string url)
        {
            var uncMatch = false;
            var delimiterIndex = url.IndexOf(SchemeDelimiter, StringComparison.Ordinal);
            if ((uncMatch = url.StartsWith(@"\\")) || (url.StartsWith("/")) || (WindowsPath.IsMatch(url)))
            {
                // Unc path
                if (uncMatch)
                {
                    var authorityEndIndex = url.IndexOf('\\', 2);
                    Authority = url.Substring(2, authorityEndIndex - 2);
                    url = url.Substring(authorityEndIndex);
                    // Path = Uri.UnescapeDataString(url);
                }
                else
                {
                    Authority = string.Empty;
                }

                url = url.Replace('\\', '/');

                Scheme = UriSchemeFile;
                Query = string.Empty;
                Fragment = string.Empty;
                Path = Uri.UnescapeDataString(url.StartsWith("/") ? url : "/" + url);
                return;
            }

            int authorityIndex;
            if (delimiterIndex == -1)
            {
                delimiterIndex = url.IndexOf(':');
                authorityIndex = delimiterIndex + 1;
                Authority = string.Empty;
                _delimiter = ":";
            }
            else
            {
                var delimiterSize = SchemeDelimiter.Length;
                authorityIndex = url.IndexOf('/', delimiterIndex + delimiterSize);
                Authority = url.Substring(delimiterIndex + delimiterSize,
                    authorityIndex - (delimiterIndex + delimiterSize));

                // this is a possible windows path without the proper tripple slash
                // file://c:/some/path.file.cs
                // We need deal with this case.
                if (Authority.IndexOf(':') > -1 || Authority.IndexOf("%3a", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    Authority = string.Empty;
                    authorityIndex = delimiterIndex + delimiterSize;
                }
            }

            Scheme = url.Substring(0, delimiterIndex);

            var fragmentIndex = url.IndexOf('#');
            if (fragmentIndex > -1)
            {
                Fragment = Uri.UnescapeDataString(url.Substring(fragmentIndex + 1));
            }
            else
            {
                Fragment = string.Empty;
                fragmentIndex = url.Length - 1;
            }

            var queryIndex = url.IndexOf('?');
            if (queryIndex > -1)
            {
                Query = Uri.UnescapeDataString(url.Substring(queryIndex + 1, fragmentIndex - (queryIndex + 1)));
            }
            else
            {
                Query = string.Empty;
                queryIndex = fragmentIndex;
            }

            Path = Uri.UnescapeDataString(url.Substring(authorityIndex, queryIndex - (authorityIndex) + 1));
        }

        /// <summary>
        /// The path
        /// </summary>
        /// <remarks>This does not contain the leading / for unix file systems.</remarks>
        public string Path { get; }

        /// <summary>
        /// The scheme of this uri
        /// </summary>
        /// <remarks>could be something other than http or https or file like custom uris the editor supports</remarks>
        public string Scheme { get; }

        /// <summary>
        /// The authority of the uri
        /// </summary>
        /// <remarks>generally is empty for language server protocol purposes</remarks>
        public string Authority { get; }

        /// <summary>
        /// The query string of the uri
        /// </summary>
        /// <remarks>generally is empty for language server protocol purposes</remarks>
        public string Query { get; }

        /// <summary>
        /// The fragment of the uri
        /// </summary>
        /// <remarks>generally is empty for language server protocol purposes</remarks>
        public string Fragment { get; }

        /// <summary>
        /// Convert the uri to a <see cref="Uri"/>
        /// </summary>
        /// <returns></returns>
        /// <remarks>This will produce a uri where asian and cyrillic characters will be encoded</remarks>
        public Uri ToUri()
        {
            if (Authority.IndexOf(':') > -1)
            {
                var parts = Authority.Split(':');
                var host = parts[0];
                var port = int.Parse(parts[1]);
                return new UriBuilder() {
                    Scheme = Scheme,
                    Host = host,
                    Port = port,
                    Path = Path,
                    Fragment = Fragment,
                    Query = Query
                }.Uri;
            }

            return new UriBuilder() {
                Scheme = Scheme,
                Host = Authority,
                Path = Path,
                Fragment = Fragment,
                Query = Query
            }.Uri;
        }

        /// <summary>
        /// Convert this uri to a proper uri string.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This will not a uri encode asian and cyrillic characters</remarks>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_stringValue))
            {
                _stringValue =
                    $"{Scheme}{_delimiter}{Authority}{Uri.EscapeUriString(Path)}{(string.IsNullOrWhiteSpace(Query) ? "" : "?" + Uri.EscapeDataString(Query))}{(string.IsNullOrWhiteSpace(Fragment) ? "" : "#" + Uri.EscapeDataString(Fragment))}";
            }

            return _stringValue;
        }

        /// <summary>
        /// Gets the file system path prefixed with / for unix platforms
        /// </summary>
        /// <returns></returns>
        /// <remarks>This will not a uri encode asian and cyrillic characters</remarks>
        public string GetFileSystemPath()
        {
            // The language server protocol represents "C:\Foo\Bar" as "file:///c:/foo/bar".
            if (Path.IndexOf(':') == -1 && !(Scheme == UriSchemeFile && !string.IsNullOrWhiteSpace(Authority)))
                return Path;
            if (!string.IsNullOrWhiteSpace(Authority))
                return $@"\\{Authority}{Path}".Replace('/', '\\');
            return Path.TrimStart('/').Replace('/', '\\');
        }

        /// <inheritdoc />
        public bool Equals(DocumentUri other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            // It's possible mac can have case insensitive file systems... we can always come back and change this.
            var comparison = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;
            return string.Equals(Path, other.Path, comparison) &&
                   string.Equals(Scheme, other.Scheme, comparison) &&
                   string.Equals(Authority, other.Authority, comparison) &&
                   string.Equals(Query, other.Query, comparison) &&
                   string.Equals(Fragment, other.Fragment, comparison);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((DocumentUri) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // It's possible mac can have case insensitive file systems... we can always come back and change this.
            var comparer = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? StringComparer.Ordinal
                : StringComparer.OrdinalIgnoreCase;
            unchecked
            {
                var hashCode = comparer.GetHashCode(Path);
                hashCode = (hashCode * 397) ^ comparer.GetHashCode(Scheme);
                hashCode = (hashCode * 397) ^ comparer.GetHashCode(Authority);
                hashCode = (hashCode * 397) ^ comparer.GetHashCode(Query);
                hashCode = (hashCode * 397) ^ comparer.GetHashCode(Fragment);
                return hashCode;
            }
        }

        /// <summary>
        /// Check if two uris are equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(DocumentUri left, DocumentUri right) => Equals(left, right);

        /// <summary>
        /// Check if two uris are not equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(DocumentUri left, DocumentUri right) => !Equals(left, right);

        /// <summary>
        /// Convert this uri into a <see cref="String"/>.
        /// </summary>
        /// <param name="uri"></param>
        /// <remarks>This is explicit because to string gives the schema string with file:// but if you want the file system you use <see cref="GetFileSystemPath()"/></remarks>
        /// <returns></returns>
        public static explicit operator string(DocumentUri uri) => uri.ToString();

        /// <summary>
        /// Convert this into a <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri"></param>
        /// <remarks>The uri class has issues with higher level utf8 characters such as asian and cyrillic characters</remarks>
        /// <returns></returns>
        public static explicit operator Uri(DocumentUri uri) => uri.ToUri();

        /// <summary>
        /// Automatically convert a string to a uri for both filesystem paths or uris in a string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static implicit operator DocumentUri(string url) => From(url);

        /// <summary>
        /// Automatically convert a uri to a document uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static implicit operator DocumentUri(Uri uri) => From(uri);

        /// <summary>
        /// Create a new document uri based on the given url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static DocumentUri Parse(string url) => new DocumentUri(url);

        /// <summary>
        /// Create a new document uri from the given <see cref="Uri"/>
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static DocumentUri From(Uri uri)
        {
            if (uri.OriginalString.IndexOf("%3A", StringComparison.OrdinalIgnoreCase) > -1)
                return new DocumentUri(uri.OriginalString);
            if (uri.Scheme == UriSchemeFile)
            {
                return new DocumentUri(uri.LocalPath);
            }

            return new DocumentUri(uri.ToString());
        }

        /// <summary>
        /// Create a new document uri from a string
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static DocumentUri From(string uri) => new DocumentUri(uri);

        /// <summary>
        /// The file scheme
        /// </summary>
        public static readonly string UriSchemeFile = Uri.UriSchemeFile;

        /// <summary>
        /// The scheme delimiter
        /// </summary>
        public static readonly string SchemeDelimiter = Uri.SchemeDelimiter;

        private string _stringValue;

        /// <summary>
        ///     Get the local file-system path for the specified document URI.
        /// </summary>
        /// <param name="textDocumentIdentifierParams">
        ///     The text document params object
        /// </param>
        /// <returns>
        ///     The file-system path, or <c>null</c> if the URI does not represent a file-system path.
        /// </returns>
        public static string GetFileSystemPath(ITextDocumentIdentifierParams textDocumentIdentifierParams) =>
            GetFileSystemPath(textDocumentIdentifierParams.TextDocument.Uri);

        /// <summary>
        ///     Get the local file-system path for the specified document URI.
        /// </summary>
        /// <param name="textDocumentIdentifier">
        ///     The text document identifier
        /// </param>
        /// <returns>
        ///     The file-system path, or <c>null</c> if the URI does not represent a file-system path.
        /// </returns>
        public static string GetFileSystemPath(TextDocumentIdentifier textDocumentIdentifier) =>
            GetFileSystemPath(textDocumentIdentifier.Uri);

        /// <summary>
        ///     Get the local file-system path for the specified document URI.
        /// </summary>
        /// <param name="documentUri">
        ///     The LSP document URI.
        /// </param>
        /// <returns>
        ///     The file-system path, or <c>null</c> if the URI does not represent a file-system path.
        /// </returns>
        public static string GetFileSystemPath(DocumentUri documentUri)
        {
            if (documentUri == null)
                throw new ArgumentNullException(nameof(documentUri));

            if (documentUri.Scheme != "file")
                return null;

            return documentUri.GetFileSystemPath();
        }

        /// <summary>
        ///     Convert a file-system path to an LSP document URI.
        /// </summary>
        /// <param name="fileSystemPath">
        ///     The file-system path.
        /// </param>
        /// <returns>
        ///     The LSP document URI.
        /// </returns>
        public static DocumentUri FromFileSystemPath(string fileSystemPath) => From(fileSystemPath);

        private sealed class DocumentUriEqualityComparer : IEqualityComparer<DocumentUri>
        {
            public bool Equals(DocumentUri x, DocumentUri y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                return x.GetType() == y.GetType() && x.Equals(y);
            }

            public int GetHashCode(DocumentUri obj) => obj.GetHashCode();
        }

        /// <summary>
        /// A default comparer that can be used for equality
        /// </summary>
        public static IEqualityComparer<DocumentUri> Comparer { get; } = new DocumentUriEqualityComparer();
    }

    static class CharCode
    {
        public const int Null = 0;
	/**
	 * The `\t` character.
	 */
	public const int Tab = 9;
	/**
	 * The `\n` character.
	 */
	public const int LineFeed = 10;
	/**
	 * The `\r` character.
	 */
	public const int CarriageReturn = 13;
	public const int Space = 32;
	/**
	 * The `!` character.
	 */
	public const int ExclamationMark = 33;
	/**
	 * The `"` character.
	 */
	public const int DoubleQuote = 34;
	/**
	 * The `#` character.
	 */
	public const int Hash = 35;
	/**
	 * The `$` character.
	 */
	public const int DollarSign = 36;
	/**
	 * The `%` character.
	 */
	public const int PercentSign = 37;
	/**
	 * The `&` character.
	 */
	public const int Ampersand = 38;
	/**
	 * The `'` character.
	 */
	public const int SingleQuote = 39;
	/**
	 * The `(` character.
	 */
	public const int OpenParen = 40;
	/**
	 * The `)` character.
	 */
	public const int CloseParen = 41;
	/**
	 * The `*` character.
	 */
	public const int Asterisk = 42;
	/**
	 * The `+` character.
	 */
	public const int Plus = 43;
	/**
	 * The `,` character.
	 */
	public const int Comma = 44;
	/**
	 * The `-` character.
	 */
	public const int Dash = 45;
	/**
	 * The `.` character.
	 */
	public const int Period = 46;
	/**
	 * The `/` character.
	 */
	public const int Slash = 47;

	public const int Digit0 = 48;
	public const int Digit1 = 49;
	public const int Digit2 = 50;
	public const int Digit3 = 51;
	public const int Digit4 = 52;
	public const int Digit5 = 53;
	public const int Digit6 = 54;
	public const int Digit7 = 55;
	public const int Digit8 = 56;
	public const int Digit9 = 57;

	/**
	 * The `:` character.
	 */
	public const int Colon = 58;
	/**
	 * The `;` character.
	 */
	public const int Semicolon = 59;
	/**
	 * The `<` character.
	 */
	public const int LessThan = 60;
	/**
	 * The `=` character.
	 */
	public const int Equals = 61;
	/**
	 * The `>` character.
	 */
	public const int GreaterThan = 62;
	/**
	 * The `?` character.
	 */
	public const int QuestionMark = 63;
	/**
	 * The `@` character.
	 */
	public const int AtSign = 64;

	public const int A = 65;
	public const int B = 66;
	public const int C = 67;
	public const int D = 68;
	public const int E = 69;
	public const int F = 70;
	public const int G = 71;
	public const int H = 72;
	public const int I = 73;
	public const int J = 74;
	public const int K = 75;
	public const int L = 76;
	public const int M = 77;
	public const int N = 78;
	public const int O = 79;
	public const int P = 80;
	public const int Q = 81;
	public const int R = 82;
	public const int S = 83;
	public const int T = 84;
	public const int U = 85;
	public const int V = 86;
	public const int W = 87;
	public const int X = 88;
	public const int Y = 89;
	public const int Z = 90;

	/**
	 * The `[` character.
	 */
	public const int OpenSquareBracket = 91;
	/**
	 * The `\` character.
	 */
	public const int Backslash = 92;
	/**
	 * The `]` character.
	 */
	public const int CloseSquareBracket = 93;
	/**
	 * The `^` character.
	 */
	public const int Caret = 94;
	/**
	 * The `_` character.
	 */
	public const int Underline = 95;
	/**
	 * The ``(`)`` character.
	 */
	public const int BackTick = 96;

	public const int a = 97;
	public const int b = 98;
	public const int c = 99;
	public const int d = 100;
	public const int e = 101;
	public const int f = 102;
	public const int g = 103;
	public const int h = 104;
	public const int i = 105;
	public const int j = 106;
	public const int k = 107;
	public const int l = 108;
	public const int m = 109;
	public const int n = 110;
	public const int o = 111;
	public const int p = 112;
	public const int q = 113;
	public const int r = 114;
	public const int s = 115;
	public const int t = 116;
	public const int u = 117;
	public const int v = 118;
	public const int w = 119;
	public const int x = 120;
	public const int y = 121;
	public const int z = 122;

	/**
	 * The `{` character.
	 */
	public const int OpenCurlyBrace = 123;
	/**
	 * The `|` character.
	 */
	public const int Pipe = 124;
	/**
	 * The `}` character.
	 */
	public const int CloseCurlyBrace = 125;
	/**
	 * The `~` character.
	 */
	public const int Tilde = 126;

	public const int U_Combining_Grave_Accent = 0x0300;								//	U+0300	Combining Grave Accent
	public const int U_Combining_Acute_Accent = 0x0301;								//	U+0301	Combining Acute Accent
	public const int U_Combining_Circumflex_Accent = 0x0302;							//	U+0302	Combining Circumflex Accent
	public const int U_Combining_Tilde = 0x0303;										//	U+0303	Combining Tilde
	public const int U_Combining_Macron = 0x0304;									//	U+0304	Combining Macron
	public const int U_Combining_Overline = 0x0305;									//	U+0305	Combining Overline
	public const int U_Combining_Breve = 0x0306;										//	U+0306	Combining Breve
	public const int U_Combining_Dot_Above = 0x0307;									//	U+0307	Combining Dot Above
	public const int U_Combining_Diaeresis = 0x0308;									//	U+0308	Combining Diaeresis
	public const int U_Combining_Hook_Above = 0x0309;								//	U+0309	Combining Hook Above
	public const int U_Combining_Ring_Above = 0x030A;								//	U+030A	Combining Ring Above
	public const int U_Combining_Double_Acute_Accent = 0x030B;						//	U+030B	Combining Double Acute Accent
	public const int U_Combining_Caron = 0x030C;										//	U+030C	Combining Caron
	public const int U_Combining_Vertical_Line_Above = 0x030D;						//	U+030D	Combining Vertical Line Above
	public const int U_Combining_Double_Vertical_Line_Above = 0x030E;				//	U+030E	Combining Double Vertical Line Above
	public const int U_Combining_Double_Grave_Accent = 0x030F;						//	U+030F	Combining Double Grave Accent
	public const int U_Combining_Candrabindu = 0x0310;								//	U+0310	Combining Candrabindu
	public const int U_Combining_Inverted_Breve = 0x0311;							//	U+0311	Combining Inverted Breve
	public const int U_Combining_Turned_Comma_Above = 0x0312;						//	U+0312	Combining Turned Comma Above
	public const int U_Combining_Comma_Above = 0x0313;								//	U+0313	Combining Comma Above
	public const int U_Combining_Reversed_Comma_Above = 0x0314;						//	U+0314	Combining Reversed Comma Above
	public const int U_Combining_Comma_Above_Right = 0x0315;							//	U+0315	Combining Comma Above Right
	public const int U_Combining_Grave_Accent_Below = 0x0316;						//	U+0316	Combining Grave Accent Below
	public const int U_Combining_Acute_Accent_Below = 0x0317;						//	U+0317	Combining Acute Accent Below
	public const int U_Combining_Left_Tack_Below = 0x0318;							//	U+0318	Combining Left Tack Below
	public const int U_Combining_Right_Tack_Below = 0x0319;							//	U+0319	Combining Right Tack Below
	public const int U_Combining_Left_Angle_Above = 0x031A;							//	U+031A	Combining Left Angle Above
	public const int U_Combining_Horn = 0x031B;										//	U+031B	Combining Horn
	public const int U_Combining_Left_Half_Ring_Below = 0x031C;						//	U+031C	Combining Left Half Ring Below
	public const int U_Combining_Up_Tack_Below = 0x031D;								//	U+031D	Combining Up Tack Below
	public const int U_Combining_Down_Tack_Below = 0x031E;							//	U+031E	Combining Down Tack Below
	public const int U_Combining_Plus_Sign_Below = 0x031F;							//	U+031F	Combining Plus Sign Below
	public const int U_Combining_Minus_Sign_Below = 0x0320;							//	U+0320	Combining Minus Sign Below
	public const int U_Combining_Palatalized_Hook_Below = 0x0321;					//	U+0321	Combining Palatalized Hook Below
	public const int U_Combining_Retroflex_Hook_Below = 0x0322;						//	U+0322	Combining Retroflex Hook Below
	public const int U_Combining_Dot_Below = 0x0323;									//	U+0323	Combining Dot Below
	public const int U_Combining_Diaeresis_Below = 0x0324;							//	U+0324	Combining Diaeresis Below
	public const int U_Combining_Ring_Below = 0x0325;								//	U+0325	Combining Ring Below
	public const int U_Combining_Comma_Below = 0x0326;								//	U+0326	Combining Comma Below
	public const int U_Combining_Cedilla = 0x0327;									//	U+0327	Combining Cedilla
	public const int U_Combining_Ogonek = 0x0328;									//	U+0328	Combining Ogonek
	public const int U_Combining_Vertical_Line_Below = 0x0329;						//	U+0329	Combining Vertical Line Below
	public const int U_Combining_Bridge_Below = 0x032A;								//	U+032A	Combining Bridge Below
	public const int U_Combining_Inverted_Double_Arch_Below = 0x032B;				//	U+032B	Combining Inverted Double Arch Below
	public const int U_Combining_Caron_Below = 0x032C;								//	U+032C	Combining Caron Below
	public const int U_Combining_Circumflex_Accent_Below = 0x032D;					//	U+032D	Combining Circumflex Accent Below
	public const int U_Combining_Breve_Below = 0x032E;								//	U+032E	Combining Breve Below
	public const int U_Combining_Inverted_Breve_Below = 0x032F;						//	U+032F	Combining Inverted Breve Below
	public const int U_Combining_Tilde_Below = 0x0330;								//	U+0330	Combining Tilde Below
	public const int U_Combining_Macron_Below = 0x0331;								//	U+0331	Combining Macron Below
	public const int U_Combining_Low_Line = 0x0332;									//	U+0332	Combining Low Line
	public const int U_Combining_Double_Low_Line = 0x0333;							//	U+0333	Combining Double Low Line
	public const int U_Combining_Tilde_Overlay = 0x0334;								//	U+0334	Combining Tilde Overlay
	public const int U_Combining_Short_Stroke_Overlay = 0x0335;						//	U+0335	Combining Short Stroke Overlay
	public const int U_Combining_Long_Stroke_Overlay = 0x0336;						//	U+0336	Combining Long Stroke Overlay
	public const int U_Combining_Short_Solidus_Overlay = 0x0337;						//	U+0337	Combining Short Solidus Overlay
	public const int U_Combining_Long_Solidus_Overlay = 0x0338;						//	U+0338	Combining Long Solidus Overlay
	public const int U_Combining_Right_Half_Ring_Below = 0x0339;						//	U+0339	Combining Right Half Ring Below
	public const int U_Combining_Inverted_Bridge_Below = 0x033A;						//	U+033A	Combining Inverted Bridge Below
	public const int U_Combining_Square_Below = 0x033B;								//	U+033B	Combining Square Below
	public const int U_Combining_Seagull_Below = 0x033C;								//	U+033C	Combining Seagull Below
	public const int U_Combining_X_Above = 0x033D;									//	U+033D	Combining X Above
	public const int U_Combining_Vertical_Tilde = 0x033E;							//	U+033E	Combining Vertical Tilde
	public const int U_Combining_Double_Overline = 0x033F;							//	U+033F	Combining Double Overline
	public const int U_Combining_Grave_Tone_Mark = 0x0340;							//	U+0340	Combining Grave Tone Mark
	public const int U_Combining_Acute_Tone_Mark = 0x0341;							//	U+0341	Combining Acute Tone Mark
	public const int U_Combining_Greek_Perispomeni = 0x0342;							//	U+0342	Combining Greek Perispomeni
	public const int U_Combining_Greek_Koronis = 0x0343;								//	U+0343	Combining Greek Koronis
	public const int U_Combining_Greek_Dialytika_Tonos = 0x0344;						//	U+0344	Combining Greek Dialytika Tonos
	public const int U_Combining_Greek_Ypogegrammeni = 0x0345;						//	U+0345	Combining Greek Ypogegrammeni
	public const int U_Combining_Bridge_Above = 0x0346;								//	U+0346	Combining Bridge Above
	public const int U_Combining_Equals_Sign_Below = 0x0347;							//	U+0347	Combining Equals Sign Below
	public const int U_Combining_Double_Vertical_Line_Below = 0x0348;				//	U+0348	Combining Double Vertical Line Below
	public const int U_Combining_Left_Angle_Below = 0x0349;							//	U+0349	Combining Left Angle Below
	public const int U_Combining_Not_Tilde_Above = 0x034A;							//	U+034A	Combining Not Tilde Above
	public const int U_Combining_Homothetic_Above = 0x034B;							//	U+034B	Combining Homothetic Above
	public const int U_Combining_Almost_Equal_To_Above = 0x034C;						//	U+034C	Combining Almost Equal To Above
	public const int U_Combining_Left_Right_Arrow_Below = 0x034D;					//	U+034D	Combining Left Right Arrow Below
	public const int U_Combining_Upwards_Arrow_Below = 0x034E;						//	U+034E	Combining Upwards Arrow Below
	public const int U_Combining_Grapheme_Joiner = 0x034F;							//	U+034F	Combining Grapheme Joiner
	public const int U_Combining_Right_Arrowhead_Above = 0x0350;						//	U+0350	Combining Right Arrowhead Above
	public const int U_Combining_Left_Half_Ring_Above = 0x0351;						//	U+0351	Combining Left Half Ring Above
	public const int U_Combining_Fermata = 0x0352;									//	U+0352	Combining Fermata
	public const int U_Combining_X_Below = 0x0353;									//	U+0353	Combining X Below
	public const int U_Combining_Left_Arrowhead_Below = 0x0354;						//	U+0354	Combining Left Arrowhead Below
	public const int U_Combining_Right_Arrowhead_Below = 0x0355;						//	U+0355	Combining Right Arrowhead Below
	public const int U_Combining_Right_Arrowhead_And_Up_Arrowhead_Below = 0x0356;	//	U+0356	Combining Right Arrowhead And Up Arrowhead Below
	public const int U_Combining_Right_Half_Ring_Above = 0x0357;						//	U+0357	Combining Right Half Ring Above
	public const int U_Combining_Dot_Above_Right = 0x0358;							//	U+0358	Combining Dot Above Right
	public const int U_Combining_Asterisk_Below = 0x0359;							//	U+0359	Combining Asterisk Below
	public const int U_Combining_Double_Ring_Below = 0x035A;							//	U+035A	Combining Double Ring Below
	public const int U_Combining_Zigzag_Above = 0x035B;								//	U+035B	Combining Zigzag Above
	public const int U_Combining_Double_Breve_Below = 0x035C;						//	U+035C	Combining Double Breve Below
	public const int U_Combining_Double_Breve = 0x035D;								//	U+035D	Combining Double Breve
	public const int U_Combining_Double_Macron = 0x035E;								//	U+035E	Combining Double Macron
	public const int U_Combining_Double_Macron_Below = 0x035F;						//	U+035F	Combining Double Macron Below
	public const int U_Combining_Double_Tilde = 0x0360;								//	U+0360	Combining Double Tilde
	public const int U_Combining_Double_Inverted_Breve = 0x0361;						//	U+0361	Combining Double Inverted Breve
	public const int U_Combining_Double_Rightwards_Arrow_Below = 0x0362;				//	U+0362	Combining Double Rightwards Arrow Below
	public const int U_Combining_Latin_Small_Letter_A = 0x0363; 						//	U+0363	Combining Latin Small Letter A
	public const int U_Combining_Latin_Small_Letter_E = 0x0364; 						//	U+0364	Combining Latin Small Letter E
	public const int U_Combining_Latin_Small_Letter_I = 0x0365; 						//	U+0365	Combining Latin Small Letter I
	public const int U_Combining_Latin_Small_Letter_O = 0x0366; 						//	U+0366	Combining Latin Small Letter O
	public const int U_Combining_Latin_Small_Letter_U = 0x0367; 						//	U+0367	Combining Latin Small Letter U
	public const int U_Combining_Latin_Small_Letter_C = 0x0368; 						//	U+0368	Combining Latin Small Letter C
	public const int U_Combining_Latin_Small_Letter_D = 0x0369; 						//	U+0369	Combining Latin Small Letter D
	public const int U_Combining_Latin_Small_Letter_H = 0x036A; 						//	U+036A	Combining Latin Small Letter H
	public const int U_Combining_Latin_Small_Letter_M = 0x036B; 						//	U+036B	Combining Latin Small Letter M
	public const int U_Combining_Latin_Small_Letter_R = 0x036C; 						//	U+036C	Combining Latin Small Letter R
	public const int U_Combining_Latin_Small_Letter_T = 0x036D; 						//	U+036D	Combining Latin Small Letter T
	public const int U_Combining_Latin_Small_Letter_V = 0x036E; 						//	U+036E	Combining Latin Small Letter V
	public const int U_Combining_Latin_Small_Letter_X = 0x036F; 						//	U+036F	Combining Latin Small Letter X

	/**
	 * Unicode Character 'LINE SEPARATOR' (U+2028)
	 * http://www.fileformat.info/info/unicode/char/2028/index.htm
	 */
	public const int LINE_SEPARATOR_2028 = 8232;

	// http://www.fileformat.info/info/unicode/category/Sk/list.htm
	public const int U_CIRCUMFLEX = 0x005E;									// U+005E	CIRCUMFLEX
	public const int U_GRAVE_ACCENT = 0x0060;								// U+0060	GRAVE ACCENT
	public const int U_DIAERESIS = 0x00A8;									// U+00A8	DIAERESIS
	public const int U_MACRON = 0x00AF;										// U+00AF	MACRON
	public const int U_ACUTE_ACCENT = 0x00B4;								// U+00B4	ACUTE ACCENT
	public const int U_CEDILLA = 0x00B8;										// U+00B8	CEDILLA
	public const int U_MODIFIER_LETTER_LEFT_ARROWHEAD = 0x02C2;				// U+02C2	MODIFIER LETTER LEFT ARROWHEAD
	public const int U_MODIFIER_LETTER_RIGHT_ARROWHEAD = 0x02C3;				// U+02C3	MODIFIER LETTER RIGHT ARROWHEAD
	public const int U_MODIFIER_LETTER_UP_ARROWHEAD = 0x02C4;				// U+02C4	MODIFIER LETTER UP ARROWHEAD
	public const int U_MODIFIER_LETTER_DOWN_ARROWHEAD = 0x02C5;				// U+02C5	MODIFIER LETTER DOWN ARROWHEAD
	public const int U_MODIFIER_LETTER_CENTRED_RIGHT_HALF_RING = 0x02D2;		// U+02D2	MODIFIER LETTER CENTRED RIGHT HALF RING
	public const int U_MODIFIER_LETTER_CENTRED_LEFT_HALF_RING = 0x02D3;		// U+02D3	MODIFIER LETTER CENTRED LEFT HALF RING
	public const int U_MODIFIER_LETTER_UP_TACK = 0x02D4;						// U+02D4	MODIFIER LETTER UP TACK
	public const int U_MODIFIER_LETTER_DOWN_TACK = 0x02D5;					// U+02D5	MODIFIER LETTER DOWN TACK
	public const int U_MODIFIER_LETTER_PLUS_SIGN = 0x02D6;					// U+02D6	MODIFIER LETTER PLUS SIGN
	public const int U_MODIFIER_LETTER_MINUS_SIGN = 0x02D7;					// U+02D7	MODIFIER LETTER MINUS SIGN
	public const int U_BREVE = 0x02D8;										// U+02D8	BREVE
	public const int U_DOT_ABOVE = 0x02D9;									// U+02D9	DOT ABOVE
	public const int U_RING_ABOVE = 0x02DA;									// U+02DA	RING ABOVE
	public const int U_OGONEK = 0x02DB;										// U+02DB	OGONEK
	public const int U_SMALL_TILDE = 0x02DC;									// U+02DC	SMALL TILDE
	public const int U_DOUBLE_ACUTE_ACCENT = 0x02DD;							// U+02DD	DOUBLE ACUTE ACCENT
	public const int U_MODIFIER_LETTER_RHOTIC_HOOK = 0x02DE;					// U+02DE	MODIFIER LETTER RHOTIC HOOK
	public const int U_MODIFIER_LETTER_CROSS_ACCENT = 0x02DF;				// U+02DF	MODIFIER LETTER CROSS ACCENT
	public const int U_MODIFIER_LETTER_EXTRA_HIGH_TONE_BAR = 0x02E5;			// U+02E5	MODIFIER LETTER EXTRA-HIGH TONE BAR
	public const int U_MODIFIER_LETTER_HIGH_TONE_BAR = 0x02E6;				// U+02E6	MODIFIER LETTER HIGH TONE BAR
	public const int U_MODIFIER_LETTER_MID_TONE_BAR = 0x02E7;				// U+02E7	MODIFIER LETTER MID TONE BAR
	public const int U_MODIFIER_LETTER_LOW_TONE_BAR = 0x02E8;				// U+02E8	MODIFIER LETTER LOW TONE BAR
	public const int U_MODIFIER_LETTER_EXTRA_LOW_TONE_BAR = 0x02E9;			// U+02E9	MODIFIER LETTER EXTRA-LOW TONE BAR
	public const int U_MODIFIER_LETTER_YIN_DEPARTING_TONE_MARK = 0x02EA;		// U+02EA	MODIFIER LETTER YIN DEPARTING TONE MARK
	public const int U_MODIFIER_LETTER_YANG_DEPARTING_TONE_MARK = 0x02EB;	// U+02EB	MODIFIER LETTER YANG DEPARTING TONE MARK
	public const int U_MODIFIER_LETTER_UNASPIRATED = 0x02ED;					// U+02ED	MODIFIER LETTER UNASPIRATED
	public const int U_MODIFIER_LETTER_LOW_DOWN_ARROWHEAD = 0x02EF;			// U+02EF	MODIFIER LETTER LOW DOWN ARROWHEAD
	public const int U_MODIFIER_LETTER_LOW_UP_ARROWHEAD = 0x02F0;			// U+02F0	MODIFIER LETTER LOW UP ARROWHEAD
	public const int U_MODIFIER_LETTER_LOW_LEFT_ARROWHEAD = 0x02F1;			// U+02F1	MODIFIER LETTER LOW LEFT ARROWHEAD
	public const int U_MODIFIER_LETTER_LOW_RIGHT_ARROWHEAD = 0x02F2;			// U+02F2	MODIFIER LETTER LOW RIGHT ARROWHEAD
	public const int U_MODIFIER_LETTER_LOW_RING = 0x02F3;					// U+02F3	MODIFIER LETTER LOW RING
	public const int U_MODIFIER_LETTER_MIDDLE_GRAVE_ACCENT = 0x02F4;			// U+02F4	MODIFIER LETTER MIDDLE GRAVE ACCENT
	public const int U_MODIFIER_LETTER_MIDDLE_DOUBLE_GRAVE_ACCENT = 0x02F5;	// U+02F5	MODIFIER LETTER MIDDLE DOUBLE GRAVE ACCENT
	public const int U_MODIFIER_LETTER_MIDDLE_DOUBLE_ACUTE_ACCENT = 0x02F6;	// U+02F6	MODIFIER LETTER MIDDLE DOUBLE ACUTE ACCENT
	public const int U_MODIFIER_LETTER_LOW_TILDE = 0x02F7;					// U+02F7	MODIFIER LETTER LOW TILDE
	public const int U_MODIFIER_LETTER_RAISED_COLON = 0x02F8;				// U+02F8	MODIFIER LETTER RAISED COLON
	public const int U_MODIFIER_LETTER_BEGIN_HIGH_TONE = 0x02F9;				// U+02F9	MODIFIER LETTER BEGIN HIGH TONE
	public const int U_MODIFIER_LETTER_END_HIGH_TONE = 0x02FA;				// U+02FA	MODIFIER LETTER END HIGH TONE
	public const int U_MODIFIER_LETTER_BEGIN_LOW_TONE = 0x02FB;				// U+02FB	MODIFIER LETTER BEGIN LOW TONE
	public const int U_MODIFIER_LETTER_END_LOW_TONE = 0x02FC;				// U+02FC	MODIFIER LETTER END LOW TONE
	public const int U_MODIFIER_LETTER_SHELF = 0x02FD;						// U+02FD	MODIFIER LETTER SHELF
	public const int U_MODIFIER_LETTER_OPEN_SHELF = 0x02FE;					// U+02FE	MODIFIER LETTER OPEN SHELF
	public const int U_MODIFIER_LETTER_LOW_LEFT_ARROW = 0x02FF;				// U+02FF	MODIFIER LETTER LOW LEFT ARROW
	public const int U_GREEK_LOWER_NUMERAL_SIGN = 0x0375;					// U+0375	GREEK LOWER NUMERAL SIGN
	public const int U_GREEK_TONOS = 0x0384;									// U+0384	GREEK TONOS
	public const int U_GREEK_DIALYTIKA_TONOS = 0x0385;						// U+0385	GREEK DIALYTIKA TONOS
	public const int U_GREEK_KORONIS = 0x1FBD;								// U+1FBD	GREEK KORONIS
	public const int U_GREEK_PSILI = 0x1FBF;									// U+1FBF	GREEK PSILI
	public const int U_GREEK_PERISPOMENI = 0x1FC0;							// U+1FC0	GREEK PERISPOMENI
	public const int U_GREEK_DIALYTIKA_AND_PERISPOMENI = 0x1FC1;				// U+1FC1	GREEK DIALYTIKA AND PERISPOMENI
	public const int U_GREEK_PSILI_AND_VARIA = 0x1FCD;						// U+1FCD	GREEK PSILI AND VARIA
	public const int U_GREEK_PSILI_AND_OXIA = 0x1FCE;						// U+1FCE	GREEK PSILI AND OXIA
	public const int U_GREEK_PSILI_AND_PERISPOMENI = 0x1FCF;					// U+1FCF	GREEK PSILI AND PERISPOMENI
	public const int U_GREEK_DASIA_AND_VARIA = 0x1FDD;						// U+1FDD	GREEK DASIA AND VARIA
	public const int U_GREEK_DASIA_AND_OXIA = 0x1FDE;						// U+1FDE	GREEK DASIA AND OXIA
	public const int U_GREEK_DASIA_AND_PERISPOMENI = 0x1FDF;					// U+1FDF	GREEK DASIA AND PERISPOMENI
	public const int U_GREEK_DIALYTIKA_AND_VARIA = 0x1FED;					// U+1FED	GREEK DIALYTIKA AND VARIA
	public const int U_GREEK_DIALYTIKA_AND_OXIA = 0x1FEE;					// U+1FEE	GREEK DIALYTIKA AND OXIA
	public const int U_GREEK_VARIA = 0x1FEF;									// U+1FEF	GREEK VARIA
	public const int U_GREEK_OXIA = 0x1FFD;									// U+1FFD	GREEK OXIA
	public const int U_GREEK_DASIA = 0x1FFE;									// U+1FFE	GREEK DASIA


	public const int U_OVERLINE = 0x203E; // Unicode Character 'OVERLINE'

    /**
	 * UTF-8 BOM
	 * Unicode Character 'ZERO WIDTH NO-BREAK SPACE' (U+FEFF)
	 * http://www.fileformat.info/info/unicode/char/feff/index.htm
	 */
    public const int UTF8_BOM = 65279;
}
class Abcd
{
    public void Space()
    {
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

bool isHighSurrogate(int charCode) {
	return (0xD800 <= charCode && charCode <= 0xDBFF);
}

bool isLowSurrogate(int charCode) {
	return (0xDC00 <= charCode && charCode <= 0xDFFF);
}

bool isLowerAsciiHex(int code) {
	return code >= CharCode.a && code <= CharCode.f;
}
bool isLowerAsciiLetter(int code) {
	return code >= CharCode.a && code <= CharCode.z;
}

bool isUpperAsciiLetter(int code) {
	return code >= CharCode.A && code <= CharCode.Z;
}

bool isAsciiLetter(int code) {
	return isLowerAsciiLetter(code) || isUpperAsciiLetter(code);
}
//#endregion

var _schemePattern = new Regex(@"^\w[\w\d+.-]*$");
var _singleSlashStart = new Regex(@"^\/");
var _doubleSlashStart = new Regex(@"^\/\/");

// void _validateUri(DocumentUri ret, bool? _strict) {
//
// 	// scheme, must be set
// 	if (!ret.scheme && _strict) {
// 		throw new Error(`[UriError]: Scheme is missing: {scheme: "", authority: "${ret.authority}", path: "${ret.path}", query: "${ret.query}", fragment: "${ret.fragment}"}`);
// 	}
//
// 	// scheme, https://tools.ietf.org/html/rfc3986#section-3.1
// 	// ALPHA *( ALPHA / DIGIT / "+" / "-" / "." )
// 	if (ret.scheme && !_schemePattern.test(ret.scheme)) {
// 		throw new Error('[UriError]: Scheme contains illegal characters.');
// 	}
//
// 	// path, http://tools.ietf.org/html/rfc3986#section-3.3
// 	// If a URI contains an authority component, then the path component
// 	// must either be empty or begin with a slash ("/") character.  If a URI
// 	// does not contain an authority component, then the path cannot begin
// 	// with two slash characters ("//").
// 	if (ret.path) {
// 		if (ret.authority) {
// 			if (!_singleSlashStart.test(ret.path)) {
// 				throw new Error('[UriError]: If a URI contains an authority component, then the path component must either be empty or begin with a slash ("/") character');
// 			}
// 		} else {
// 			if (_doubleSlashStart.test(ret.path)) {
// 				throw new Error('[UriError]: If a URI does not contain an authority component, then the path cannot begin with two slash characters ("//")');
// 			}
// 		}
// 	}
// }

// for a while we allowed uris *without* schemes and this is the migration
// for them, e.g. an uri without scheme and without strict-mode warns and falls
// back to the file-scheme. that should cause the least carnage and still be a
// clear warning
string _schemeFix(string scheme, bool _strict) {
	if (!string.IsNullOrWhiteSpace(scheme) && !_strict) {
		return "file";
	}
	return scheme;
}

const string _empty = "";
const char _slash = '/';
const string _strSlash = "/";
var _regexp = new Regex(@"^(([^:/?#]+?):)?(\/\/([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?");

// implements a bit of https://tools.ietf.org/html/rfc3986#section-5
string _referenceResolution(string scheme, string path) {

	// the slash-character is our 'default base' as we don't
	// support constructing URIs relative to other URIs. This
	// also means that we alter and potentially break paths.
	// see https://tools.ietf.org/html/rfc3986#section-5.1.4
	switch (scheme) {
		case "https":
		case "http":
		case "file":
			if (!string.IsNullOrWhiteSpace(path)) {
				path = _strSlash;
			} else if (path[0] != _slash) {
				path = _slash + path;
			}
			break;
	}
	return path;
}
}
}

/**
 * Uniform Resource Identifier (URI) http://tools.ietf.org/html/rfc3986.
 * This class is a simple parser which creates the basic component parts
 * (http://tools.ietf.org/html/rfc3986#section-3) with minimal validation
 * and encoding.
 *
 * ```txt
 *       foo://example.com:8042/over/there?name=ferret#nose
 *       \_/   \______________/\_________/ \_________/ \__/
 *        |           |            |            |        |
 *     scheme     authority       path        query   fragment
 *        |   _____________________|__
 *       / \ /                        \
 *       urn:example:animal:ferret:nose
 * ```
 */
class URI {


    /**
	 * @internal
	 */
    protected URI(string scheme, string authority, string path, string query, string fragment, bool? _strict)
    {

    }

  //   /**
	 // * @internal
	 // */
  //   protected URI(schemeOrData: string | UriComponents, authority?: string, path?: string, query?: string, fragment?: string, _strict: boolean = false) {
  //
  //       if (typeof schemeOrData === 'object') {
  //           this.scheme = schemeOrData.scheme || _empty;
  //           this.authority = schemeOrData.authority || _empty;
  //           this.path = schemeOrData.path || _empty;
  //           this.query = schemeOrData.query || _empty;
  //           this.fragment = schemeOrData.fragment || _empty;
  //           // no validation because it's this URI
  //           // that creates uri components.
  //           // _validateUri(this);
  //       } else {
  //           this.scheme = _schemeFix(schemeOrData, _strict);
  //           this.authority = authority || _empty;
  //           this.path = _referenceResolution(this.scheme, path || _empty);
  //           this.query = query || _empty;
  //           this.fragment = fragment || _empty;
  //
  //           _validateUri(this, _strict);
  //       }
  //   }
	/**
	 * scheme is the 'http' part of 'http://www.msft.com/some/path?query#fragment'.
	 * The part before the first colon.
	 */
	public string scheme { get;}

	/**
	 * authority is the 'www.msft.com' part of 'http://www.msft.com/some/path?query#fragment'.
	 * The part between the first double slashes and the next slash.
	 */
    public string authority { get;}

	/**
	 * path is the '/some/path' part of 'http://www.msft.com/some/path?query#fragment'.
	 */
    public string path { get;}

	/**
	 * query is the 'query' part of 'http://www.msft.com/some/path?query#fragment'.
	 */
    public string query { get;}

	/**
	 * fragment is the 'fragment' part of 'http://www.msft.com/some/path?query#fragment'.
	 */
    public string fragment { get;}

	// ---- filesystem path -----------------------

    /**
	 * Returns a string representing the corresponding file system path of this URI.
	 * Will handle UNC paths, normalizes windows drive letters to lower-case, and uses the
	 * platform specific path separator.
	 *
	 * * Will *not* validate the path for invalid characters and semantics.
	 * * Will *not* look at the scheme of this URI.
	 * * The result shall *not* be used for display purposes but for accessing a file on disk.
	 *
	 *
	 * The *difference* to `URI#path` is the use of the platform specific separator and the handling
	 * of UNC paths. See the below sample of a file-uri with an authority (UNC path).
	 *
	 * ```ts
		const u = URI.parse('file://server/c$/folder/file.txt')
		u.authority === 'server'
		u.path === '/shares/c$/file.txt'
		u.fsPath === '\\server\c$\folder\file.txt'
	```
	 *
	 * Using `URI#path` to read a file (using fs-apis) would not be enough because parts of the path,
	 * namely the server name, would be missing. Therefore `URI#fsPath` exists - it's sugar to ease working
	 * with URIs that represent files on disk (`file` scheme).
	 */
    public string fsPath => uriToFsPath(this, false);

	// ---- parse & validate ------------------------

	/**
	 * Creates a new URI from a string, e.g. `http://www.msft.com/some/path`,
	 * `file:///usr/home`, or `scheme:with/path`.
	 *
	 * @param value A string which represents an URI (see `URI#toString`).
	 */
	static URI parse(string value, bool _strict = false)  {
		const match = _regexp.exec(value);
		if (!match) {
			return new _URI(_empty, _empty, _empty, _empty, _empty);
		}
		return new _URI(
			match[2] || _empty,
			percentDecode(match[4] || _empty),
			percentDecode(match[5] || _empty),
			percentDecode(match[7] || _empty),
			percentDecode(match[9] || _empty),
			_strict
		);
	}

	/**
	 * Creates a new URI from a file system path, e.g. `c:\my\files`,
	 * `/usr/home`, or `\\server\share\some\path`.
	 *
	 * The *difference* between `URI#parse` and `URI#file` is that the latter treats the argument
	 * as path, not as stringified-uri. E.g. `URI.file(path)` is **not the same as**
	 * `URI.parse('file://' + path)` because the path might contain characters that are
	 * interpreted (# and ?). See the following sample:
	 * ```ts
	const good = URI.file('/coding/c#/project1');
	good.scheme === 'file';
	good.path === '/coding/c#/project1';
	good.fragment === '';
	const bad = URI.parse('file://' + '/coding/c#/project1');
	bad.scheme === 'file';
	bad.path === '/coding/c'; // path is now broken
	bad.fragment === '/project1';
	```
	 *
	 * @param path A file system path (see `URI#fsPath`)
	 */
	static URI file(string path) {

		let authority = _empty;

		// normalize to fwd-slashes on windows,
		// on other systems bwd-slashes are valid
		// filename character, eg /f\oo/ba\r.txt
		if (isWindows) {
			path = path.replace(/\\/g, _slash);
		}

		// check for authority as used in UNC shares
		// or use the path as given
		if (path[0] === _slash && path[1] === _slash) {
			const idx = path.indexOf(_slash, 2);
			if (idx === -1) {
				authority = path.substring(2);
				path = _slash;
			} else {
				authority = path.substring(2, idx);
				path = path.substring(idx) || _slash;
			}
		}

		return new _URI('file', authority, path, _empty, _empty);
	}

	// ---- printing/externalize ---------------------------

	/**
	 * Creates a string representation for this URI. It's guaranteed that calling
	 * `URI.parse` with the result of this function creates an URI which is equal
	 * to this URI.
	 *
	 * * The result shall *not* be used for display purposes but for externalization or transport.
	 * * The result will be encoded using the percentage encoding and encoding happens mostly
	 * ignore the scheme-specific encoding rules.
	 *
	 * @param skipEncoding Do not encode the result, default is `false`
	 */
	public override string ToString() {
		return _asFormatted(this, false);
	}

    public string ToUnencodedString()
    {
        return _asFormatted(this, true);
    }
}

const _pathSepMarker = isWindows ? 1 : undefined;

// eslint-disable-next-line @typescript-eslint/class-name-casing
class _URI extends URI {

	_formatted: string | null = null;
	_fsPath: string | null = null;

	get fsPath(): string {
		if (!this._fsPath) {
			this._fsPath = uriToFsPath(this, false);
		}
		return this._fsPath;
	}

	toString(skipEncoding: boolean = false): string {
		if (!skipEncoding) {
			if (!this._formatted) {
				this._formatted = _asFormatted(this, false);
			}
			return this._formatted;
		} else {
			// we don't cache that
			return _asFormatted(this, true);
		}
	}

}

// reserved characters: https://tools.ietf.org/html/rfc3986#section-2.2
const encodeTable: { [ch: number]: string } = {
	[CharCode.Colon]: '%3A', // gen-delims
	[CharCode.Slash]: '%2F',
	[CharCode.QuestionMark]: '%3F',
	[CharCode.Hash]: '%23',
	[CharCode.OpenSquareBracket]: '%5B',
	[CharCode.CloseSquareBracket]: '%5D',
	[CharCode.AtSign]: '%40',

	[CharCode.ExclamationMark]: '%21', // sub-delims
	[CharCode.DollarSign]: '%24',
	[CharCode.Ampersand]: '%26',
	[CharCode.SingleQuote]: '%27',
	[CharCode.OpenParen]: '%28',
	[CharCode.CloseParen]: '%29',
	[CharCode.Asterisk]: '%2A',
	[CharCode.Plus]: '%2B',
	[CharCode.Comma]: '%2C',
	[CharCode.Semicolon]: '%3B',
	[CharCode.Equals]: '%3D',

	[CharCode.Space]: '%20',
};

function encodeURIComponentFast(uriComponent: string, allowSlash: boolean): string {
	let res: string | undefined = undefined;
	let nativeEncodePos = -1;

	for (let pos = 0; pos < uriComponent.length; pos++) {
		const code = uriComponent.charCodeAt(pos);

		// unreserved characters: https://tools.ietf.org/html/rfc3986#section-2.3
		if (
			(code >= CharCode.a && code <= CharCode.z)
			|| (code >= CharCode.A && code <= CharCode.Z)
			|| (code >= CharCode.Digit0 && code <= CharCode.Digit9)
			|| code === CharCode.Dash
			|| code === CharCode.Period
			|| code === CharCode.Underline
			|| code === CharCode.Tilde
			|| (allowSlash && code === CharCode.Slash)
		) {
			// check if we are delaying native encode
			if (nativeEncodePos !== -1) {
				res += encodeURIComponent(uriComponent.substring(nativeEncodePos, pos));
				nativeEncodePos = -1;
			}
			// check if we write into a new string (by default we try to return the param)
			if (res !== undefined) {
				res += uriComponent.charAt(pos);
			}

		} else {
			// encoding needed, we need to allocate a new string
			if (res === undefined) {
				res = uriComponent.substr(0, pos);
			}

			// check with default table first
			const escaped = encodeTable[code];
			if (escaped !== undefined) {

				// check if we are delaying native encode
				if (nativeEncodePos !== -1) {
					res += encodeURIComponent(uriComponent.substring(nativeEncodePos, pos));
					nativeEncodePos = -1;
				}

				// append escaped variant to result
				res += escaped;

			} else if (nativeEncodePos === -1) {
				// use native encode only when needed
				nativeEncodePos = pos;
			}
		}
	}

	if (nativeEncodePos !== -1) {
		res += encodeURIComponent(uriComponent.substring(nativeEncodePos));
	}

	return res !== undefined ? res : uriComponent;
}

function encodeURIComponentMinimal(path: string): string {
	let res: string | undefined = undefined;
	for (let pos = 0; pos < path.length; pos++) {
		const code = path.charCodeAt(pos);
		if (code === CharCode.Hash || code === CharCode.QuestionMark) {
			if (res === undefined) {
				res = path.substr(0, pos);
			}
			res += encodeTable[code];
		} else {
			if (res !== undefined) {
				res += path[pos];
			}
		}
	}
	return res !== undefined ? res : path;
}

/**
 * Compute `fsPath` for the given uri
 */
export function uriToFsPath(uri: URI, keepDriveLetterCasing: boolean): string {

	let value: string;
	if (uri.authority && uri.path.length > 1 && uri.scheme === 'file') {
		// unc path: file://shares/c$/far/boo
		value = `//${uri.authority}${uri.path}`;
	} else if (
		uri.path.charCodeAt(0) === CharCode.Slash
		&& (uri.path.charCodeAt(1) >= CharCode.A && uri.path.charCodeAt(1) <= CharCode.Z || uri.path.charCodeAt(1) >= CharCode.a && uri.path.charCodeAt(1) <= CharCode.z)
		&& uri.path.charCodeAt(2) === CharCode.Colon
	) {
		if (!keepDriveLetterCasing) {
			// windows drive letter: file:///c:/far/boo
			value = uri.path[1].toLowerCase() + uri.path.substr(2);
		} else {
			value = uri.path.substr(1);
		}
	} else {
		// other path
		value = uri.path;
	}
	if (isWindows) {
		value = value.replace(/\//g, '\\');
	}
	return value;
}

/**
 * Create the external version of a uri
 */
function _asFormatted(uri: URI, skipEncoding: boolean): string {

	const encoder = !skipEncoding
		? encodeURIComponentFast
		: encodeURIComponentMinimal;

	let res = '';
	let { scheme, authority, path, query, fragment } = uri;
	if (scheme) {
		res += scheme;
		res += ':';
	}
	if (authority || scheme === 'file') {
		res += _slash;
		res += _slash;
	}
	if (authority) {
		let idx = authority.indexOf('@');
		if (idx !== -1) {
			// <user>@<auth>
			const userinfo = authority.substr(0, idx);
			authority = authority.substr(idx + 1);
			idx = userinfo.indexOf(':');
			if (idx === -1) {
				res += encoder(userinfo, false);
			} else {
				// <user>:<pass>@<auth>
				res += encoder(userinfo.substr(0, idx), false);
				res += ':';
				res += encoder(userinfo.substr(idx + 1), false);
			}
			res += '@';
		}
		authority = authority.toLowerCase();
		idx = authority.indexOf(':');
		if (idx === -1) {
			res += encoder(authority, false);
		} else {
			// <auth>:<port>
			res += encoder(authority.substr(0, idx), false);
			res += authority.substr(idx);
		}
	}
	if (path) {
		// lower-case windows drive letters in /C:/fff or C:/fff
		if (path.length >= 3 && path.charCodeAt(0) === CharCode.Slash && path.charCodeAt(2) === CharCode.Colon) {
			const code = path.charCodeAt(1);
			if (code >= CharCode.A && code <= CharCode.Z) {
				path = `/${String.fromCharCode(code + 32)}:${path.substr(3)}`; // "/c:".length === 3
			}
		} else if (path.length >= 2 && path.charCodeAt(1) === CharCode.Colon) {
			const code = path.charCodeAt(0);
			if (code >= CharCode.A && code <= CharCode.Z) {
				path = `${String.fromCharCode(code + 32)}:${path.substr(2)}`; // "/c:".length === 3
			}
		}
		// encode the rest of the path
		res += encoder(path, true);
	}
	if (query) {
		res += '?';
		res += encoder(query, false);
	}
	if (fragment) {
		res += '#';
		res += !skipEncoding ? encodeURIComponentFast(fragment, false) : fragment;
	}
	return res;
}

// --- decode

function decodeURIComponentGraceful(str: string): string {
	try {
		return decodeURIComponent(str);
	} catch {
		if (str.length > 3) {
			return str.substr(0, 3) + decodeURIComponentGraceful(str.substr(3));
		} else {
			return str;
		}
	}
}

const _rEncodedAsHex = /(%[0-9A-Za-z][0-9A-Za-z])+/g;

function percentDecode(str: string): string {
	if (!str.match(_rEncodedAsHex)) {
		return str;
	}
	return str.replace(_rEncodedAsHex, (match) => decodeURIComponentGraceful(match));
}
}

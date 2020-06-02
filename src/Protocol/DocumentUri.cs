using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// Uniform Resource Identifier (URI) http://tools.ietf.org/html/rfc3986.
    /// This class is a simple parser which creates the basic component parts
    /// (http://tools.ietf.org/html/rfc3986#section-3) with minimal validation
    /// and encoding.
    ///
    /// ```txt
    ///       foo://example.com:8042/over/there?name=ferret#nose
    ///       \_/   \______________/\_________/ \_________/ \__/
    ///        |           |            |            |        |
    ///     scheme     authority       path        query   fragment
    ///        |   _____________________|__
    ///       / \ /                        \
    ///       urn:example:animal:ferret:nose
    /// ```
    /// </summary>
    /// <summary>
    /// This class describes a document uri as defined by https://microsoft.github.io/language-server-protocol/specifications/specification-current/#uri
    /// </summary>
    /// <remarks>This exists because of some non-standard serialization in vscode around uris and .NET's behavior when deserializing those uris</remarks>
    public partial class DocumentUri : IEquatable<DocumentUri>
    {
        /// <summary>
        /// scheme is the "http' part of 'http://www.msft.com/some/path?query#fragment".
        /// The part before the first colon.
        /// </summary>
        public string Scheme { get; }

        /// <summary>
        /// authority is the "www.msft.com' part of 'http://www.msft.com/some/path?query#fragment".
        /// The part between the first double slashes and the next slash.
        /// </summary>
        public string Authority { get; }

        /// <summary>
        /// path is the "/some/path' part of 'http://www.msft.com/some/path?query#fragment".
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// query is the "query' part of 'http://www.msft.com/some/path?query#fragment".
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// fragment is the "fragment' part of 'http://www.msft.com/some/path?query#fragment".
        /// </summary>
        public string Fragment { get; }

        /// <summary>
        /// Convert the uri to a <see cref="Uri"/>
        /// </summary>
        /// <returns></returns>
        /// <remarks>This will produce a uri where asian and cyrillic characters will be encoded</remarks>
        public Uri ToUri()
        {
            if (Authority.IndexOf(":") > -1)
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

        // ---- printing/externalize ---------------------------

        /// <summary>
        /// Creates a string representation for this URI. It's guaranteed that calling
        /// `URI.parse` with the result of this function creates an URI which is equal
        /// to this URI.
        ///
        /// * The result shall *not* be used for display purposes but for externalization or transport.
        /// * The result will be encoded using the percentage encoding and encoding happens mostly
        /// ignore the scheme-specific encoding rules.
        ///
        /// @param skipEncoding Do not encode the result, default is `false`
        /// </summary>
        public override string ToString()
        {
            return _asFormatted(this, false);
        }

        public string ToUnencodedString()
        {
            return _asFormatted(this, true);
        }

        /// <summary>
        /// Gets the file system path prefixed with / for unix platforms
        /// </summary>
        /// <returns></returns>
        /// <remarks>This will not a uri encode asian and cyrillic characters</remarks>
        public string GetFileSystemPath() => UriToFsPath(this, false);

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
        /// Deconstruct the document uri into it's different parts
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="authority"></param>
        /// <param name="path"></param>
        /// <param name="query"></param>
        /// <param name="fragment"></param>
        /// <returns></returns>
        public void Deconstruct(out string scheme, out string authority, out string path, out string query,
            out string fragment)
        {
            scheme = Scheme;
            authority = Authority;
            path =  Path;
            query = Query;
            fragment = Fragment;
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
        /// Convert this uri into a <see cref="string"/>.
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
        /// Create a new document uri from the given <see cref="Uri"/>
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static DocumentUri From(Uri uri)
        {
            if (uri.OriginalString.IndexOf(EncodeTable[CharCode.Colon], StringComparison.OrdinalIgnoreCase) > -1)
                return From(uri.OriginalString);
            if (uri.Scheme == Uri.UriSchemeFile)
            {
                return From(uri.LocalPath);
            }

            return From(uri.ToString());
        }

        /// <summary>
        /// Create a new document uri from a string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static DocumentUri From(string url)
        {
            if (string.IsNullOrWhiteSpace (url))
            {
                throw new UriFormatException("Given uri is null or empty");
            }
            if (url.StartsWith(@"\\") || (url.StartsWith("/")) || WindowsPath.IsMatch(url))
            {
                return File(url);
            }

            return Parse(url);
        }

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

            if (documentUri.Scheme != Uri.UriSchemeFile)
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
        public static DocumentUri FromFileSystemPath(string fileSystemPath) => File(fileSystemPath);

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

        // for a while we allowed uris *without* schemes and this is the migration
        // for them, e.g. an uri without scheme and without strict-mode warns and falls
        // back to the file-scheme. that should cause the least carnage and still be a
        // clear warning

        // implements a bit of https://tools.ietf.org/html/rfc3986#section-5


        // reserved characters: https://tools.ietf.org/html/rfc3986#section-2.2

        // --- decode

        /// <summary>
        /// @internal
        /// </summary>
        public DocumentUri(string scheme, string authority, string path, string query, string fragment,
            bool? strict = null)
        {
            Scheme = SchemeFix(scheme, strict);
            Authority = authority ?? Empty;
            Path = ReferenceResolution(Scheme, path ?? Empty);
            Query = query ?? Empty;
            Fragment = fragment ?? Empty;

            _validateUri(this, strict);
        }

        // ---- parse & validate ------------------------

        /// <summary>
        /// Creates a new URI from a string, e.g. `http://www.msft.com/some/path`,
        /// `file:///usr/home`, or `scheme:with/path`.
        ///
        /// @param value A string which represents an URI (see `URI#toString`).
        /// </summary>
        public static DocumentUri Parse(string value, bool strict = false)
        {
            if (string.IsNullOrWhiteSpace (value))
            {
                throw new UriFormatException("Given uri is null or empty");
            }
            var match = Regexp.Match(value);
            if (!match.Success)
            {
                return new DocumentUri(Empty, Empty, Empty, Empty, Empty);
            }

            return new DocumentUri(
                match.Groups[2].Value ?? Empty,
                PercentDecode(match.Groups[4].Value ?? Empty),
                PercentDecode(match.Groups[5].Value ?? Empty),
                PercentDecode(match.Groups[7].Value ?? Empty),
                PercentDecode(match.Groups[9].Value ?? Empty),
                strict
            );
        }

        /// <summary>
        /// Creates a new URI from a file system path, e.g. `c:\my\files`,
        /// `/usr/home`, or `\\server\share\some\path`.
        ///
        /// The *difference* between `URI#parse` and `URI#file` is that the latter treats the argument
        /// as path, not as stringified-uri. E.g. `URI.file(path)` is **not the same as**
        /// `URI.parse("file://" + path)` because the path might contain characters that are
        /// interpreted (# and ?). See the following sample:
        ///
        /// @param path A file system path (see `URI#fsPath`)
        /// </summary>
        public static DocumentUri File(string path)
        {
            if (string.IsNullOrWhiteSpace (path))
            {
                throw new UriFormatException("Given path is null or empty");
            }
            var authority = Empty;

            // normalize to fwd-slashes on windows,
            // on other systems bwd-slashes are valid
            // filename character, eg /f\oo/ba\r.txt
            if (path[0] != Slash)
            {
                path = path.Replace('\\', Slash);
            }

            // check for authority as used in UNC shares
            // or use the path as given
            if (path[0] == Slash && path[1] == Slash)
            {
                var idx = path.IndexOf(Slash, 2);
                if (idx == -1)
                {
                    authority = path.Substring(2);
                    path = StrSlash;
                }
                else
                {
                    authority = path.Substring(2, idx - 2);
                    path = path.Substring(idx);
                    if (string.IsNullOrWhiteSpace(path)) path = StrSlash;
                }
            }

            if (path.IndexOf("%3A", StringComparison.OrdinalIgnoreCase) > -1)
            {
                path = path.Replace("%3a", ":").Replace("%3A", ":");
            }

            return new DocumentUri("file", authority, path, Empty, Empty, null);
        }

        public DocumentUri With(DocumentUriComponents components)
        {
            return new DocumentUri(
                components.Scheme ?? Scheme,
                components.Authority ?? Authority,
                components.Path ?? Path,
                components.Query ?? Query,
                components.Fragment ?? Fragment
            );
        }

        public static DocumentUri From(DocumentUriComponents components)
        {
            return new DocumentUri(
                components.Scheme ?? string.Empty,
                components.Authority ?? string.Empty,
                components.Path ?? string.Empty,
                components.Query ?? string.Empty,
                components.Fragment ?? string.Empty
            );
        }
    }
}

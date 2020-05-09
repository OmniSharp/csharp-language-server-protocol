using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// This class describes a document uri as defined by https://microsoft.github.io/language-server-protocol/specifications/specification-current/#uri
    /// </summary>
    /// <remarks>This exists because of some non-standard serialization in vscode around uris and .NET's behavior when deserializing those uris</remarks>
    public class DocumentUri : IEquatable<DocumentUri>
    {
        /// <summary>
        /// Create a new document uri
        /// </summary>
        /// <param name="url"></param>
        public DocumentUri(string url)
        {
            var delimiterIndex = url.IndexOf(SchemeDelimiter, StringComparison.Ordinal);
            if (delimiterIndex == -1)
            {
                // Unc path
                if (url.StartsWith("\\\\"))
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

            Scheme = url.Substring(0, delimiterIndex);

            var authorityIndex = url.IndexOf('/', delimiterIndex + SchemeDelimiter.Length);
            Authority = url.Substring(delimiterIndex + SchemeDelimiter.Length,
                authorityIndex - (delimiterIndex + SchemeDelimiter.Length));

            // this is a possible windows path without the proper tripple slash
            // file://c:/some/path.file.cs
            // We need deal with this case.
            if (Authority.IndexOf(':') > -1 || Authority.IndexOf("%3a", StringComparison.OrdinalIgnoreCase) > -1)
            {
                Authority = string.Empty;
                authorityIndex = delimiterIndex + SchemeDelimiter.Length;
            }

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
        public override string ToString() =>
            $"{Scheme}{SchemeDelimiter}{Authority}{Path}{(string.IsNullOrWhiteSpace(Query) ? "" : "?" + Query)}{(string.IsNullOrWhiteSpace(Fragment) ? "" : "#" + Fragment)}";

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
                return $"\\\\{Authority}{Path}".Replace('/', '\\');
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
}

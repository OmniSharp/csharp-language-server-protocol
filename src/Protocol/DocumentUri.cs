using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static System.IO.Path;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public class DocumentUri : IEquatable<DocumentUri>
    {
        public DocumentUri(string url)
        {
            var delimiterIndex = url.IndexOf(SchemeDelimiter, StringComparison.Ordinal);
            if (delimiterIndex == -1)
            {
                url = Uri.UnescapeDataString(url);
                if (!IsPathRooted(url))
                    throw new ArgumentException($"Path '{url}' is not an absolute path.", nameof(url));

                if (DirectorySeparatorChar == '\\')
                    url = url.Replace(DirectorySeparatorChar, AltDirectorySeparatorChar);
                Scheme = UriSchemeFile;
                Authority = string.Empty;
                Query = string.Empty;
                Fragment = string.Empty;
                Path = Uri.UnescapeDataString(url).TrimStart('/');

                return;
            }

            Scheme = url.Substring(0, delimiterIndex);

            var authorityIndex = url.IndexOf('/', delimiterIndex + SchemeDelimiter.Length);
            Authority = url.Substring(delimiterIndex + SchemeDelimiter.Length,
                authorityIndex - (delimiterIndex + SchemeDelimiter.Length));

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

            Path = Uri.UnescapeDataString(url.Substring(authorityIndex + 1, queryIndex - (authorityIndex))).TrimStart('/');
        }


        public string Path { get; }
        public string Scheme { get; }
        public string Authority { get; }
        public string Query { get; }
        public string Fragment { get; }

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

        public override string ToString() =>
            $"{Scheme}{SchemeDelimiter}{Authority}/{Path}{(string.IsNullOrWhiteSpace(Query) ? "" : "?" + Query)}{(string.IsNullOrWhiteSpace(Fragment) ? "" : "#" + Fragment)}";


        public string GetFileSystemPath()
        {
            // The language server protocol represents "C:\Foo\Bar" as "file:///c:/foo/bar".
            return Path.IndexOf(':') == -1 ? "/" + Path : Combine(Path.TrimStart('/')).Replace('/', '\\');
        }

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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((DocumentUri) obj);
        }

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

        public static bool operator ==(DocumentUri left, DocumentUri right) => Equals(left, right);

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

        public static implicit operator DocumentUri(string url) => From(url);

        public static implicit operator DocumentUri(Uri uri) => From(uri);

        public static DocumentUri Parse(string url) => new DocumentUri(url);

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

        public static DocumentUri From(string uri) => new DocumentUri(uri);

        public static readonly string UriSchemeFile = Uri.UriSchemeFile;
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
        public static string GetFileSystemPath(ITextDocumentIdentifierParams textDocumentIdentifierParams) => GetFileSystemPath(textDocumentIdentifierParams.TextDocument.Uri);

        /// <summary>
        ///     Get the local file-system path for the specified document URI.
        /// </summary>
        /// <param name="textDocumentIdentifier">
        ///     The text document identifier
        /// </param>
        /// <returns>
        ///     The file-system path, or <c>null</c> if the URI does not represent a file-system path.
        /// </returns>
        public static string GetFileSystemPath(TextDocumentIdentifier textDocumentIdentifier) => GetFileSystemPath(textDocumentIdentifier.Uri);

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

        public static IEqualityComparer<DocumentUri> Comparer { get; } = new DocumentUriEqualityComparer();
    }
}

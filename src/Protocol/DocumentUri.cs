using System;
using System.IO;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    ///     Helper methods for working with LSP document URIs.
    /// </summary>
    public static class DocumentUri
    {
        /// <summary>
        ///     Get the local file-system path for the specified document URI.
        /// </summary>
        /// <param name="textDocumentIdentifierParams">
        ///     The text document params object
        /// </param>
        /// <returns>
        ///     The file-system path, or <c>null</c> if the URI does not represent a file-system path.
        /// </returns>
        public static string GetFileSystemPath(ITextDocumentIdentifierParams textDocumentIdentifierParams)
        {
            return GetFileSystemPath(textDocumentIdentifierParams.TextDocument.Uri);
        }

        /// <summary>
        ///     Get the local file-system path for the specified document URI.
        /// </summary>
        /// <param name="textDocumentIdentifier">
        ///     The text document identifier
        /// </param>
        /// <returns>
        ///     The file-system path, or <c>null</c> if the URI does not represent a file-system path.
        /// </returns>
        public static string GetFileSystemPath(TextDocumentIdentifier textDocumentIdentifier)
        {
            return GetFileSystemPath(textDocumentIdentifier.Uri);
        }

        /// <summary>
        ///     Get the local file-system path for the specified document URI.
        /// </summary>
        /// <param name="documentUri">
        ///     The LSP document URI.
        /// </param>
        /// <returns>
        ///     The file-system path, or <c>null</c> if the URI does not represent a file-system path.
        /// </returns>
        public static string GetFileSystemPath(Uri documentUri)
        {
            if (documentUri == null)
                throw new ArgumentNullException(nameof(documentUri));

            if (documentUri.Scheme != "file")
                return null;

            // The language server protocol represents "C:\Foo\Bar" as "file:///c:/foo/bar".
            string fileSystemPath = Uri.UnescapeDataString(documentUri.AbsolutePath);
            if (Path.DirectorySeparatorChar == '\\')
            {
                if (fileSystemPath.StartsWith("/"))
                    fileSystemPath = fileSystemPath.Substring(1);

                fileSystemPath = fileSystemPath.Replace('/', '\\');
            }

            return NormalizePath(fileSystemPath);
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
        public static Uri FromFileSystemPath(string fileSystemPath)
        {
            fileSystemPath = NormalizePath(fileSystemPath);

            if (string.IsNullOrWhiteSpace(fileSystemPath))
                throw new ArgumentException(
                    "Argument cannot be null, empty, or entirely composed of whitespace: 'fileSystemPath'.",
                    nameof(fileSystemPath));

            if (!Path.IsPathRooted(fileSystemPath))
                throw new ArgumentException($"Path '{fileSystemPath}' is not an absolute path.",
                    nameof(fileSystemPath));

            if (Path.DirectorySeparatorChar == '\\')
                return new Uri("file:///" + fileSystemPath.Replace('\\', '/'));

            return NormalizeUri(new Uri("file://" + fileSystemPath));
        }

        /// <summary>
        /// Vscode has a special uri serialization this will normalize that.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Uri NormalizeUri(Uri uri)
        {
            // On windows of the Uri contains %3a local path
            // doesn't come out as a proper windows path
            return uri.Segments[1].IndexOf("%3a", StringComparison.OrdinalIgnoreCase) > -1
                ? new Uri(uri.AbsoluteUri.Replace("%3a", ":").Replace("%3A", ":"))
                : uri;
        }

        /// <summary>
        /// Vscode has a special uri serialization this will normalize that.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            if (path.IndexOf("%3a", StringComparison.OrdinalIgnoreCase) > -1)
                return path.Replace("%3a", ":").Replace("%3A", ":");
            return path;
        }
    }
}

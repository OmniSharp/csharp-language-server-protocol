using System;
using System.IO;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Client.Clients
{
    /// <summary>
    ///     Client for the LSP Text Document API.
    /// </summary>
    public partial class TextDocumentClient
    {
        /// <summary>
        ///     Notify the language server that the client has opened a text document.
        /// </summary>
        /// <param name="filePath">
        ///     The full path to the text document.
        /// </param>
        /// <param name="languageId">
        ///     The document language type (e.g. "xml").
        /// </param>
        /// <param name="version">
        ///     The document version (optional).
        /// </param>
        /// <remarks>
        ///     Will automatically populate the document text, if available.
        /// </remarks>
        public void DidOpen(string filePath, string languageId, int version = 0)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(filePath)}.", nameof(filePath));

            string text = null;
            if (File.Exists(filePath))
                text = File.ReadAllText(filePath);

            DidOpen(
                DocumentUri.FromFileSystemPath(filePath),
                languageId,
                text,
                version
            );
        }

        /// <summary>
        ///     Notify the language server that the client has opened a text document.
        /// </summary>
        /// <param name="filePath">
        ///     The full file-system path of the text document.
        /// </param>
        /// <param name="languageId">
        ///     The document language type (e.g. "xml").
        /// </param>
        /// <param name="text">
        ///     The document text (pass null to have the language server retrieve the text itself).
        /// </param>
        /// <param name="version">
        ///     The document version (optional).
        /// </param>
        public void DidOpen(string filePath, string languageId, string text, int version = 0)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(filePath)}.", nameof(filePath));

            DidOpen(DocumentUri.FromFileSystemPath(filePath), languageId, text, version);
        }

        /// <summary>
        ///     Notify the language server that the client has opened a text document.
        /// </summary>
        /// <param name="documentUri">
        ///     The document URI.
        /// </param>
        /// <param name="languageId">
        ///     The document language type (e.g. "xml").
        /// </param>
        /// <param name="text">
        ///     The document text.
        /// </param>
        /// <param name="version">
        ///     The document version (optional).
        /// </param>
        public void DidOpen(DocumentUri documentUri, string languageId, string text, int version = 0)
        {
            if (documentUri == null)
                throw new ArgumentNullException(nameof(documentUri));

            Client.SendNotification(TextDocumentNames.DidOpen, new DidOpenTextDocumentParams
            {
                TextDocument = new TextDocumentItem
                {
                    Text = text,
                    LanguageId = languageId,
                    Version = version,
                    Uri = documentUri
                }
            });
        }

        /// <summary>
        ///     Notify the language server that the client has changed a text document.
        /// </summary>
        /// <param name="filePath">
        ///     The full path to the text document.
        /// </param>
        /// <param name="languageId">
        ///     The document language type (e.g. "xml").
        /// </param>
        /// <param name="version">
        ///     The document version (optional).
        /// </param>
        /// <remarks>
        ///     This style of notification is used when the client does not support partial updates (i.e. one or more updates with an associated range).
        ///
        ///     Will automatically populate the document text, if available.
        /// </remarks>
        public void DidChange(string filePath, string languageId, int version = 0)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(filePath)}.", nameof(filePath));

            string text = null;
            if (File.Exists(filePath))
                text = File.ReadAllText(filePath);

            DidChange(
                DocumentUri.FromFileSystemPath(filePath),
                languageId,
                text,
                version
            );
        }

        /// <summary>
        ///     Notify the language server that the client has changed a text document.
        /// </summary>
        /// <param name="filePath">
        ///     The full file-system path of the text document.
        /// </param>
        /// <param name="languageId">
        ///     The document language type (e.g. "xml").
        /// </param>
        /// <param name="text">
        ///     The document text (pass null to have the language server retrieve the text itself).
        /// </param>
        /// <param name="version">
        ///     The document version (optional).
        /// </param>
        /// <remarks>
        ///     This style of notification is used when the client does not support partial updates (i.e. one or more updates with an associated range).
        /// </remarks>
        public void DidChange(string filePath, string languageId, string text, int version = 0)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(filePath)}.", nameof(filePath));

            DidChange(DocumentUri.FromFileSystemPath(filePath), languageId, text, version);
        }

        /// <summary>
        ///     Notify the language server that the client has changed a text document.
        /// </summary>
        /// <param name="documentUri">
        ///     The document URI.
        /// </param>
        /// <param name="languageId">
        ///     The document language type (e.g. "xml").
        /// </param>
        /// <param name="text">
        ///     The document text.
        /// </param>
        /// <param name="version">
        ///     The document version (optional).
        /// </param>
        /// <remarks>
        ///     This style of notification is used when the client does not support partial updates (i.e. one or more updates with an associated range).
        /// </remarks>
        public void DidChange(DocumentUri documentUri, string languageId, string text, int version = 0)
        {
            if (documentUri == null)
                throw new ArgumentNullException(nameof(documentUri));

            Client.SendNotification(TextDocumentNames.DidChange, new DidChangeTextDocumentParams
            {
                TextDocument = new VersionedTextDocumentIdentifier
                {
                    Version = version,
                    Uri = documentUri
                },
                ContentChanges = new TextDocumentContentChangeEvent[]
                {
                    new TextDocumentContentChangeEvent
                    {
                        Text = text
                    }
                }
            });
        }

        /// <summary>
        ///     Notify the language server that the client has closed a text document.
        /// </summary>
        /// <param name="filePath">
        ///     The full file-system path of the text document.
        /// </param>
        public void DidClose(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(filePath)}.", nameof(filePath));

            DidClose(
                DocumentUri.FromFileSystemPath(filePath)
            );
        }

        /// <summary>
        ///     Notify the language server that the client has closed a text document.
        /// </summary>
        /// <param name="documentUri">
        ///     The document URI.
        /// </param>
        public void DidClose(DocumentUri documentUri)
        {
            if (documentUri == null)
                throw new ArgumentNullException(nameof(documentUri));

            Client.SendNotification(TextDocumentNames.DidClose, new DidCloseTextDocumentParams
            {
                TextDocument = new TextDocumentItem
                {
                    Uri = documentUri
                }
            });
        }

        /// <summary>
        ///     Notify the language server that the client has saved a text document.
        /// </summary>
        /// <param name="filePath">
        ///     The full file-system path of the text document.
        /// </param>
        public void DidSave(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(filePath)}.", nameof(filePath));

            DidSave(
                DocumentUri.FromFileSystemPath(filePath)
            );
        }

        /// <summary>
        ///     Notify the language server that the client has saved a text document.
        /// </summary>
        /// <param name="documentUri">
        ///     The document URI.
        /// </param>
        public void DidSave(DocumentUri documentUri)
        {
            if (documentUri == null)
                throw new ArgumentNullException(nameof(documentUri));

            Client.SendNotification(TextDocumentNames.DidSave, new DidSaveTextDocumentParams
            {
                TextDocument = new TextDocumentItem
                {
                    Uri = documentUri
                }
            });
        }
    }
}

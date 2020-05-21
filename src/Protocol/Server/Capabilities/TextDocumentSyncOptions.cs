using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class TextDocumentSyncOptions : ITextDocumentSyncOptions
    {
        /// <summary>
        ///  Open and close notifications are sent to the server.
        /// </summary>
        [Optional]
        public bool OpenClose { get; set; }
        /// <summary>
        ///  Change notificatins are sent to the server. See TextDocumentSyncKind.None, TextDocumentSyncKind.Full
        ///  and TextDocumentSyncKindIncremental.
        /// </summary>
        [Optional]
        public TextDocumentSyncKind Change { get; set; }
        /// <summary>
        ///  Will save notifications are sent to the server.
        /// </summary>
        [Optional]
        public bool WillSave { get; set; }
        /// <summary>
        ///  Will save wait until requests are sent to the server.
        /// </summary>
        [Optional]
        public bool WillSaveWaitUntil { get; set; }
        /// <summary>
        ///  Save notifications are sent to the server.
        /// </summary>
        [Optional]
        public SaveOptions Save { get; set; }

        public static TextDocumentSyncOptions Of(IEnumerable<ITextDocumentSyncOptions> options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            var change = TextDocumentSyncKind.None;
            if (options.Any(x => x.Change != TextDocumentSyncKind.None))
            {
                change = options
                        .Where(x => x.Change != TextDocumentSyncKind.None)
                        .Min<ITextDocumentSyncOptions, TextDocumentSyncKind>(z => z.Change);
            }
            return new TextDocumentSyncOptions()
            {
                OpenClose = options.Any(z => z.OpenClose),
                Change = change,
                WillSave = options.Any(z => z.WillSave),
                WillSaveWaitUntil = options.Any(z => z.WillSaveWaitUntil),
                Save = new SaveOptions()
                {
                    IncludeText = options.Any(z => z.Save?.IncludeText == true)
                }
            };
        }
    }
}

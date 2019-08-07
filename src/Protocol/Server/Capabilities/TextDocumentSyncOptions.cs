using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

        public static Func<IEnumerable<ITextDocumentSyncOptions>, TextDocumentSyncOptions> Of(TextDocumentSyncOptions @default)
        {
            return options =>
            {
                var change = @default.Change;
                if (@default.Change == TextDocumentSyncKind.None && options.Any())
                {
                    change = @default.Change > 0 ? @default.Change : options
                            .Where(x => x.Change != TextDocumentSyncKind.None)
                            .Min(z => z.Change);
                }
                return new TextDocumentSyncOptions()
                {
                    OpenClose = @default.OpenClose || options.Any(z => z.OpenClose),
                    Change = change,
                    WillSave = @default.WillSave || options.Any(z => z.WillSave),
                    WillSaveWaitUntil = @default.WillSaveWaitUntil || options.Any(z => z.WillSaveWaitUntil),
                    Save = new SaveOptions()
                    {
                        IncludeText = @default.Save?.IncludeText ?? options.Any(z => z.Save?.IncludeText == true)
                    }
                };
            };
        }
    }
}

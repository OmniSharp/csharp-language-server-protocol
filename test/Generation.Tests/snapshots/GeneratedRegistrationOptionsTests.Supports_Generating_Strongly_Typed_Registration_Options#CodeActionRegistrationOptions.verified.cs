﻿//HintName: CodeActionRegistrationOptions.cs
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [RegistrationOptionsKey(nameof(ServerCapabilities.CodeActionProvider))]
    [RegistrationOptionsConverterAttribute(typeof(CodeActionRegistrationOptionsConverter))]
    public partial class CodeActionRegistrationOptions : OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions, OmniSharp.Extensions.LanguageServer.Protocol.Models.ITextDocumentRegistrationOptions, OmniSharp.Extensions.LanguageServer.Protocol.Models.IWorkDoneProgressOptions
    {
        public TextDocumentSelector? DocumentSelector { get; set; }

        [Optional]
        public bool WorkDoneProgress { get; set; }

        class CodeActionRegistrationOptionsConverter : RegistrationOptionsConverterBase<CodeActionRegistrationOptions, StaticOptions>
        {
            public CodeActionRegistrationOptionsConverter()
            {
            }

            public override StaticOptions Convert(CodeActionRegistrationOptions source)
            {
                return new StaticOptions
                {
                    CodeActionKinds = source.CodeActionKinds,
                    ResolveProvider = source.ResolveProvider,
                    WorkDoneProgress = source.WorkDoneProgress
                };
            }
        }

        [RegistrationOptionsKey(nameof(ServerCapabilities.CodeActionProvider))]
        public partial class StaticOptions : OmniSharp.Extensions.LanguageServer.Protocol.Models.IWorkDoneProgressOptions
        {
            /// <summary>
            /// CodeActionKinds that this server may return.
            ///
            /// The list of kinds may be generic, such as `CodeActionKind.Refactor`, or the server
            /// may list out every specific kind they provide.
            /// </summary>
            [Optional]
            public Container<CodeActionKind>? CodeActionKinds { get; set; } = new Container<CodeActionKind>();

            /// <summary>
            /// The server provides support to resolve additional
            /// information for a code action.
            ///
            /// @since 3.16.0
            /// </summary>
            [Optional]
            public bool ResolveProvider { get; set; }

            [Optional]
            public bool WorkDoneProgress { get; set; }
        }
    }
}
#nullable restore

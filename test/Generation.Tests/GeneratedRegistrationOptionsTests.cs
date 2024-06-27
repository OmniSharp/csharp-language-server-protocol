using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Generators;
using Xunit;

namespace Generation.Tests
{
    public class GeneratedRegistrationOptionsTests
    {
        [Fact]
        public async Task Supports_Generating_Strongly_Typed_WorkDone_Registration_Options()
        {
            var source = @"
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Test
{
    [GenerateRegistrationOptions(nameof(ServerCapabilities.WorkspaceSymbolProvider), SupportsWorkDoneProgress = true)]
    public partial class WorkspaceSymbolRegistrationOptions { }
}
";
            await Verify(await GenerationHelpers.GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Generating_Strongly_Typed_WorkDone_Registration_Options_Interface()
        {
            var source = @"
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Test
{
    [GenerateRegistrationOptions(nameof(ServerCapabilities.WorkspaceSymbolProvider))]
    public partial class WorkspaceSymbolRegistrationOptions : IWorkDoneProgressOptions { }
}
";

            await Verify(await GenerationHelpers.GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Generating_Strongly_Typed_Registration_Options()
        {
            var source = @"
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
    [GenerateRegistrationOptions(nameof(ServerCapabilities.CodeActionProvider), SupportsTextDocumentSelector = true, SupportsWorkDoneProgress = true]
    public partial class CodeActionRegistrationOptions
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
    }
}
#nullable restore";
            await Verify(await GenerationHelpers.GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Generating_Strongly_Typed_Registration_Options_With_Converters()
        {
            var source = @"
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

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [GenerateRegistrationOptions(nameof(ServerCapabilities.CodeActionProvider)]
    [RegistrationOptionsConverter(typeof(CodeActionRegistrationOptionsConverter))]
    public partial class CodeActionRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
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

        class CodeActionRegistrationOptionsConverter : RegistrationOptionsConverterBase<CodeActionRegistrationOptions, StaticCodeActionRegistrationOptions>
        {
            private readonly IHandlersManager _handlersManager;

            public Converter(IHandlersManager handlersManager)
            {
                _handlersManager = handlersManager;
            }

            public override StaticCodeActionRegistrationOptions Convert(CodeActionRegistrationOptions source)
            {
                return new StaticCodeActionRegistrationOptions {
                    CodeActionKinds = source.CodeActionKinds,
                    ResolveProvider = source.ResolveProvider || _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(ICodeActionResolveHandler)),
                    WorkDoneProgress = source.WorkDoneProgress,
                };
            }
        }
    }
}
#nullable restore";

            await Verify(await GenerationHelpers.GenerateAll(source));
        }
    }
}

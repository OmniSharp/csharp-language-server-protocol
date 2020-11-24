using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Generators;
using TestingUtils;

namespace Generation.Tests
{
    public class GeneratedRegistrationOptionsTests
    {
        [FactWithSkipOn(SkipOnPlatform.Windows)]
        public async Task Supports_Generating_Strongly_Typed_WorkDone_Registration_Options()
        {
            var source = @"
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Test
{
    [GenerateRegistrationOptions(nameof(ServerCapabilities.WorkspaceSymbolProvider), SupportsWorkDoneProgress = true)]
    public partial class WorkspaceSymbolRegistrationOptions { }
}
";
            var expected = @"
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

#nullable enable
namespace Test
{
    [RegistrationOptionsConverterAttribute(typeof(WorkspaceSymbolRegistrationOptionsConverter))]
    public partial class WorkspaceSymbolRegistrationOptions : OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions, OmniSharp.Extensions.LanguageServer.Protocol.Models.IWorkDoneProgressOptions
    {
        [Optional]
        public bool WorkDoneProgress
        {
            get;
            set;
        }

        class WorkspaceSymbolRegistrationOptionsConverter : RegistrationOptionsConverterBase<WorkspaceSymbolRegistrationOptions, StaticOptions>
        {
            public WorkspaceSymbolRegistrationOptionsConverter(): base(nameof(ServerCapabilities.WorkspaceSymbolProvider))
            {
            }

            public override StaticOptions Convert(WorkspaceSymbolRegistrationOptions source)
            {
                return new StaticOptions{WorkDoneProgress = source.WorkDoneProgress};
            }
        }

        public partial class StaticOptions : OmniSharp.Extensions.LanguageServer.Protocol.Models.IWorkDoneProgressOptions
        {
            [Optional]
            public bool WorkDoneProgress
            {
                get;
                set;
            }
        }
    }
}
#nullable restore";
            await GenerationHelpers.AssertGeneratedAsExpected<RegistrationOptionsGenerator>(source, expected);
        }
        [FactWithSkipOn(SkipOnPlatform.Windows)]
        public async Task Supports_Generating_Strongly_Typed_WorkDone_Registration_Options_Interface()
        {
            var source = @"
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Test
{
    [GenerateRegistrationOptions(nameof(ServerCapabilities.WorkspaceSymbolProvider))]
    public partial class WorkspaceSymbolRegistrationOptions : IWorkDoneProgressOptions { }
}
";

            var expected = @"
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

#nullable enable
namespace Test
{
    [RegistrationOptionsConverterAttribute(typeof(WorkspaceSymbolRegistrationOptionsConverter))]
    public partial class WorkspaceSymbolRegistrationOptions : OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions
    {
        [Optional]
        public bool WorkDoneProgress
        {
            get;
            set;
        }

        class WorkspaceSymbolRegistrationOptionsConverter : RegistrationOptionsConverterBase<WorkspaceSymbolRegistrationOptions, StaticOptions>
        {
            public WorkspaceSymbolRegistrationOptionsConverter(): base(nameof(ServerCapabilities.WorkspaceSymbolProvider))
            {
            }

            public override StaticOptions Convert(WorkspaceSymbolRegistrationOptions source)
            {
                return new StaticOptions{WorkDoneProgress = source.WorkDoneProgress};
            }
        }

        public partial class StaticOptions : IWorkDoneProgressOptions
        {
            [Optional]
            public bool WorkDoneProgress
            {
                get;
                set;
            }
        }
    }
}
#nullable restore";
            await GenerationHelpers.AssertGeneratedAsExpected<RegistrationOptionsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [GenerateRegistrationOptions(nameof(ServerCapabilities.CodeActionProvider), SupportsDocumentSelector = true, SupportsWorkDoneProgress = true]
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
        public bool ResolveProvider { get; set; }
    }
}
#nullable restore";

            var expected = @"
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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [RegistrationOptionsConverterAttribute(typeof(CodeActionRegistrationOptionsConverter))]
    public partial class CodeActionRegistrationOptions : OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions, OmniSharp.Extensions.LanguageServer.Protocol.Models.ITextDocumentRegistrationOptions, OmniSharp.Extensions.LanguageServer.Protocol.Models.IWorkDoneProgressOptions
    {
        public DocumentSelector? DocumentSelector
        {
            get;
            set;
        }

        [Optional]
        public bool WorkDoneProgress
        {
            get;
            set;
        }

        class CodeActionRegistrationOptionsConverter : RegistrationOptionsConverterBase<CodeActionRegistrationOptions, StaticOptions>
        {
            public CodeActionRegistrationOptionsConverter(): base(nameof(ServerCapabilities.CodeActionProvider))
            {
            }

            public override StaticOptions Convert(CodeActionRegistrationOptions source)
            {
                return new StaticOptions{CodeActionKinds = source.CodeActionKinds, ResolveProvider = source.ResolveProvider, WorkDoneProgress = source.WorkDoneProgress};
            }
        }

        public partial class StaticOptions : OmniSharp.Extensions.LanguageServer.Protocol.Models.IWorkDoneProgressOptions
        {
            /// <summary>
            /// CodeActionKinds that this server may return.
            ///
            /// The list of kinds may be generic, such as `CodeActionKind.Refactor`, or the server
            /// may list out every specific kind they provide.
            /// </summary>
            [Optional]
            public Container<CodeActionKind>? CodeActionKinds
            {
                get;
                set;
            }

            = new Container<CodeActionKind>();
            /// <summary>
            /// The server provides support to resolve additional
            /// information for a code action.
            ///
            /// @since 3.16.0
            /// </summary>
            public bool ResolveProvider
            {
                get;
                set;
            }

            [Optional]
            public bool WorkDoneProgress
            {
                get;
                set;
            }
        }
    }
}
#nullable restore";
            await GenerationHelpers.AssertGeneratedAsExpected<RegistrationOptionsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

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
        public bool ResolveProvider { get; set; }

        class CodeActionRegistrationOptionsConverter : RegistrationOptionsConverterBase<CodeActionRegistrationOptions, StaticCodeActionRegistrationOptions>
        {
            private readonly IHandlersManager _handlersManager;

            public Converter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.CodeActionProvider))
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

            var expected = @"
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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    public partial class CodeActionRegistrationOptions : OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions
    {
        public DocumentSelector? DocumentSelector
        {
            get;
            set;
        }

        [Optional]
        public bool WorkDoneProgress
        {
            get;
            set;
        }

        public partial class StaticOptions : IWorkDoneProgressOptions
        {
            /// <summary>
            /// CodeActionKinds that this server may return.
            ///
            /// The list of kinds may be generic, such as `CodeActionKind.Refactor`, or the server
            /// may list out every specific kind they provide.
            /// </summary>
            [Optional]
            public Container<CodeActionKind>? CodeActionKinds
            {
                get;
                set;
            }

            = new Container<CodeActionKind>();
            /// <summary>
            /// The server provides support to resolve additional
            /// information for a code action.
            ///
            /// @since 3.16.0
            /// </summary>
            public bool ResolveProvider
            {
                get;
                set;
            }

            [Optional]
            public bool WorkDoneProgress
            {
                get;
                set;
            }
        }
    }
}
#nullable restore";
            await GenerationHelpers.AssertGeneratedAsExpected<RegistrationOptionsGenerator>(source, expected);
        }
    }
}

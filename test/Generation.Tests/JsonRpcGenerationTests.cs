using System.Threading.Tasks;
using FluentAssertions;
using OmniSharp.Extensions.JsonRpc.Generators;
using Xunit;
using Xunit.Sdk;
using static Generation.Tests.GenerationHelpers;

namespace Generation.Tests
{
    public class JsonRpcGenerationTests
    {
        [Fact]
        public async Task Supports_Generating_Notifications_And_Infers_Direction_ExitHandler()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Serial, Method(GeneralNames.Exit, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods]
    public interface IExitHandler : IJsonRpcNotificationHandler<ExitParams>
    {
    }
}";

            await Verify(GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Generating_Generic_Response_Types()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Serial]
    [Method(WorkspaceNames.ExecuteCommand, Direction.ClientToServer)]
    [
        GenerateHandler(""OmniSharp.Extensions.LanguageServer.Protocol.Workspace"" /*, AllowDerivedRequests = true*/),
        GenerateHandlerMethods,
        GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))
            ]
        [RegistrationOptions(typeof(ExecuteCommandRegistrationOptions)), Capability(typeof(ExecuteCommandCapability))]
    public partial record ExecuteCommandParams<T> : IRequest<T>, IWorkDoneProgressParams, IExecuteCommandParams
    {
        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        public string Command { get; init; }

        /// <summary>
        /// Arguments that the command should be invoked with.
        /// </summary>
        [Optional]
        public JArray? Arguments { get; init; }
    }
}";

            await Verify(GenerateAll(source));
        }

        [Fact]
        public async Task Should_Report_Diagnostic_If_Missing_Information()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Test
{
    [Serial, Method(GeneralNames.Exit, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods]
    public interface IExitHandler : IJsonRpcNotificationHandler<ExitParams>
    {
    }
}";

            var a = () => AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, "");
            await a.Should().ThrowAsync<EmptyException>().WithMessage("*Could not infer the request router(s)*");
            await a.Should().ThrowAsync<EmptyException>("cache").WithMessage("*Could not infer the request router(s)*");
            await Verify(GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Generating_Notifications_And_Infers_Direction_CapabilitiesHandler()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events.Test
{
    [Parallel, Method(EventNames.Capabilities, Direction.ServerToClient), GenerateHandlerMethods, GenerateRequestMethods]
    public interface ICapabilitiesHandler : IJsonRpcNotificationHandler<CapabilitiesEvent> { }
}";

            await Verify(GenerateAll(source));
        }


        [Fact]
        public async Task Supports_Generating_Notifications_ExitHandler()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Test
{
    [Serial, Method(GeneralNames.Exit, Direction.ClientToServer), GenerateHandlerMethods(typeof(ILanguageServerRegistry)), GenerateRequestMethods(typeof(ILanguageClient))]
    public interface IExitHandler : IJsonRpcNotificationHandler<ExitParams>
    {
    }
}";

            await Verify(GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Generating_Notifications_And_Infers_Direction_DidChangeTextHandler()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Serial, Method(TextDocumentNames.DidChange, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods]
    public interface IDidChangeTextDocumentHandler : IJsonRpcNotificationHandler<DidChangeTextDocumentParams>,
        IRegistration<TextDocumentChangeRegistrationOptions, TextSynchronizationCapability>
    { }
}";

            await Verify(GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Generating_Notifications_And_Infers_Direction_FoldingRangeHandler()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Parallel, Method(TextDocumentNames.FoldingRange, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IFoldingRangeHandler : IJsonRpcRequestHandler<FoldingRangeRequestParam, Container<FoldingRange>>,
        IRegistration<FoldingRangeRegistrationOptions, FoldingRangeCapability>
    {
    }
}";

            await Verify(GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Generating_Requests_And_Infers_Direction()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Parallel, Method(TextDocumentNames.Definition, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods, Obsolete(""This is obsolete"")]
    public interface IDefinitionHandler : IJsonRpcRequestHandler<DefinitionParams, LocationOrLocationLinks>, IRegistration<DefinitionRegistrationOptions, DefinitionCapability> { }
}";

            await Verify(GenerationHelpers.GenerateAll(source));
        }


        [Fact]
        public async Task Supports_Generating_Requests()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace Test
{
    [Parallel, Method(TextDocumentNames.Definition, Direction.ClientToServer), GenerateHandlerMethods(typeof(ILanguageServerRegistry)), GenerateRequestMethods(typeof(ITextDocumentLanguageClient))]
    public interface IDefinitionHandler : IJsonRpcRequestHandler<DefinitionParams, LocationOrLocationLinks>, IRegistration<DefinitionRegistrationOptions, DefinitionCapability> { }
}";

            await Verify(GenerationHelpers.GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Custom_Method_Names()
        {
            var source = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Test;

namespace Test
{
    [Serial, Method(GeneralNames.Initialize, Direction.ClientToServer), GenerateHandlerMethods(typeof(ILanguageServerRegistry), MethodName = ""OnLanguageProtocolInitialize""), GenerateRequestMethods(typeof(ITextDocumentLanguageClient), MethodName = ""RequestLanguageProtocolInitialize"")]
    public interface ILanguageProtocolInitializeHandler : IJsonRpcRequestHandler<InitializeParams, InitializeResult> {}
}";
            await Verify(GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Allow_Derived_Requests()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using System.Collections.Generic;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    public class AttachResponse { }
    [Method(""attach"", Direction.ClientToServer)]
    [GenerateHandler(AllowDerivedRequests = true), GenerateHandlerMethods, GenerateRequestMethods]
    public class AttachRequestArguments: IRequest<AttachResponse> { }
}";
            await Verify(GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Allow_Derived_Requests_Nullable()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using System.Collections.Generic;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    public class LaunchResponse { }
    [Method(""launch"", Direction.ClientToServer)]
    [GenerateHandler(AllowDerivedRequests = true), GenerateHandlerMethods, GenerateRequestMethods]
    public class LaunchRequestArguments: IRequest<LaunchResponse?> { }
}";
            await Verify(GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Allows_Nullable_Responses()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using System.Collections.Generic;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    public class AttachResponse { }
    [Method(""attach"", Direction.ClientToServer)]
    [GenerateHandler(AllowDerivedRequests = true), GenerateHandlerMethods, GenerateRequestMethods]
    public class AttachRequestArguments: IRequest<AttachResponse?> { }
}";
            await Verify(GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Allow_Generic_Types()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using System.Collections.Generic;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    public class AttachResponse { }
    [Method(""attach"", Direction.ClientToServer)]
    [GenerateHandler(AllowDerivedRequests = true), GenerateHandlerMethods, GenerateRequestMethods]
    public class AttachRequestArguments: IRequest<AttachResponse> { }
}";
            await Verify(GenerateAll(source));
        }

        [Fact]
        public async Task Supports_Params_Type_As_Source()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using System.Collections.Generic;
using MediatR;
using RenameRegistrationOptions = OmniSharp.Extensions.LanguageServer.Protocol.Models.RenameRegistrationOptions;
using RenameCapability = OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.RenameCapability;
using ITextDocumentIdentifierParams = OmniSharp.Extensions.LanguageServer.Protocol.Models.ITextDocumentIdentifierParams;
using WorkspaceEdit = OmniSharp.Extensions.LanguageServer.Protocol.Models.WorkspaceEdit;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Bogus
{
    [Parallel]
    [Method(TextDocumentNames.Rename, Direction.ClientToServer)]
    [GenerateHandler(""OmniSharp.Extensions.LanguageServer.Protocol.Bogus.Handlers""), GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    [RegistrationOptions(typeof(RenameRegistrationOptions))]
    [Capability(typeof(RenameCapability))]
    public partial class RenameParams : ITextDocumentIdentifierParams, IRequest<WorkspaceEdit?>
    {
    }
}";
            await Verify(GenerateAll(source));
        }
    }
}

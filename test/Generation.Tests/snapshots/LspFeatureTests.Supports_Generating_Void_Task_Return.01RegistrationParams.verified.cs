//HintName: RegistrationParams.cs
using Lsp.Tests.Integration.Fixtures;
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
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Parallel, Method(ClientNames.RegisterCapability, Direction.ServerToClient)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IRegisterCapabilityHandler : IJsonRpcRequestHandler<RegistrationParams, MediatR.Unit>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class RegisterCapabilityHandlerBase : AbstractHandlers.Request<RegistrationParams, MediatR.Unit>, IRegisterCapabilityHandler
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class RegisterCapabilityExtensions
    {
        public static ILanguageClientRegistry OnRegisterCapability(this ILanguageClientRegistry registry, Func<RegistrationParams, Task> handler) => registry.AddHandler(ClientNames.RegisterCapability, new DelegatingHandlers.Request<RegistrationParams>(HandlerAdapter.Adapt<RegistrationParams>(handler)));
        public static ILanguageClientRegistry OnRegisterCapability(this ILanguageClientRegistry registry, Func<RegistrationParams, CancellationToken, Task> handler) => registry.AddHandler(ClientNames.RegisterCapability, new DelegatingHandlers.Request<RegistrationParams>(HandlerAdapter.Adapt<RegistrationParams>(handler)));
    }
#nullable restore
}
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Generators;
using TestingUtils;
using Xunit;

namespace Generation.Tests
{
    public class LspFeatureTests
    {
//        [Fact(Skip = "for testing"]
        [Fact]
        public async Task Supports_Params_Type_As_Source()
        {
            var source = FeatureFixture.ReadSource("Workspace.WorkspaceSymbolsFeature.cs");
            var expected = @"
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
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel, Method(WorkspaceNames.WorkspaceSymbol, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IWorkspaceSymbolsHandler : IJsonRpcRequestHandler<WorkspaceSymbolParams, Container<SymbolInformation>?>, IRegistration<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class WorkspaceSymbolsHandlerBase : AbstractHandlers.Request<WorkspaceSymbolParams, Container<SymbolInformation>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>, IWorkspaceSymbolsHandler
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class WorkspaceSymbolsPartialHandlerBase : AbstractHandlers.PartialResults<WorkspaceSymbolParams, Container<SymbolInformation>?, SymbolInformation, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>, IWorkspaceSymbolsHandler
    {
        protected WorkspaceSymbolsPartialHandlerBase(System.Guid id, IProgressManager progressManager): base(progressManager, Container<SymbolInformation>.From)
        {
        }
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class WorkspaceSymbolsExtensions
    {
        public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry, Func<WorkspaceSymbolParams, Task<Container<SymbolInformation>?>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, new LanguageProtocolDelegatingHandlers.Request<WorkspaceSymbolParams, Container<SymbolInformation>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(HandlerAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, Container<SymbolInformation>?>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry, Func<WorkspaceSymbolParams, CancellationToken, Task<Container<SymbolInformation>?>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, new LanguageProtocolDelegatingHandlers.Request<WorkspaceSymbolParams, Container<SymbolInformation>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(HandlerAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, Container<SymbolInformation>?>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry, Func<WorkspaceSymbolParams, WorkspaceSymbolCapability, CancellationToken, Task<Container<SymbolInformation>?>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, new LanguageProtocolDelegatingHandlers.Request<WorkspaceSymbolParams, Container<SymbolInformation>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(HandlerAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, Container<SymbolInformation>?>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveWorkspaceSymbols(this ILanguageServerRegistry registry, Action<WorkspaceSymbolParams, IObserver<IEnumerable<SymbolInformation>>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, _ => new LanguageProtocolDelegatingHandlers.PartialResults<WorkspaceSymbolParams, Container<SymbolInformation>?, SymbolInformation, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(PartialAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, SymbolInformation>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<SymbolInformation>.From));
        }

        public static ILanguageServerRegistry ObserveWorkspaceSymbols(this ILanguageServerRegistry registry, Action<WorkspaceSymbolParams, IObserver<IEnumerable<SymbolInformation>>, CancellationToken> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, _ => new LanguageProtocolDelegatingHandlers.PartialResults<WorkspaceSymbolParams, Container<SymbolInformation>?, SymbolInformation, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(PartialAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, SymbolInformation>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<SymbolInformation>.From));
        }

        public static ILanguageServerRegistry ObserveWorkspaceSymbols(this ILanguageServerRegistry registry, Action<WorkspaceSymbolParams, IObserver<IEnumerable<SymbolInformation>>, WorkspaceSymbolCapability, CancellationToken> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, _ => new LanguageProtocolDelegatingHandlers.PartialResults<WorkspaceSymbolParams, Container<SymbolInformation>?, SymbolInformation, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(PartialAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, SymbolInformation>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<SymbolInformation>.From));
        }

        public static IRequestProgressObservable<IEnumerable<SymbolInformation>, Container<SymbolInformation>?> RequestWorkspaceSymbols(this ITextDocumentLanguageClient mediator, WorkspaceSymbolParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, value => new Container<SymbolInformation>(value), cancellationToken);
        public static IRequestProgressObservable<IEnumerable<SymbolInformation>, Container<SymbolInformation>?> RequestWorkspaceSymbols(this ILanguageClient mediator, WorkspaceSymbolParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, value => new Container<SymbolInformation>(value), cancellationToken);
    }
#nullable restore
}";
            await GenerationHelpers.AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [Fact]
        public async Task Supports_Generating_Custom_Language_Extensions()
        {
            var source = @"
// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated a code generator.
// </auto-generated>
// ------------------------------------------------------------------------------

using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

#nullable enable
namespace Lsp.Tests.Integration.Fixtures
{
    [Parallel, Method(""tests/run"", Direction.ClientToServer)]
    [
        GenerateHandler,
        GenerateHandlerMethods(typeof(ILanguageServerRegistry)),
        GenerateRequestMethods(typeof(ILanguageClient))
    ]
    [RegistrationOptions(typeof(UnitTestRegistrationOptions)), Capability(typeof(UnitTestCapability))]
    public partial class UnitTest : IRequest
    {
        public string Name { get; set; } = null!;
    }

    [Parallel, Method(""tests/discover"", Direction.ClientToServer)]
    [
        GenerateHandler,
        GenerateHandlerMethods(typeof(ILanguageServerRegistry)),
        GenerateRequestMethods(typeof(ILanguageClient))
    ]
    [RegistrationOptions(typeof(UnitTestRegistrationOptions)), Capability(typeof(UnitTestCapability))]
    public partial class DiscoverUnitTestsParams : IPartialItemsRequest<Container<UnitTest>, UnitTest>, IWorkDoneProgressParams
    {
        public ProgressToken? PartialResultToken { get; set; } = null!;
        public ProgressToken? WorkDoneToken { get; set; } = null!;
    }

    [CapabilityKey(""workspace"", ""unitTests"")]
    public partial class UnitTestCapability : DynamicCapability
    {
        public string Property { get; set; } = null!;
    }

    [GenerateRegistrationOptions(""unitTestDiscovery"")]
    public partial class UnitTestRegistrationOptions : IWorkDoneProgressOptions
    {
        [Optional] public bool SupportsDebugging { get; set; }
    }
}
#nullable restore";

            var expectedOptions = @"
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

#nullable enable
namespace Lsp.Tests.Integration.Fixtures
{
    [RegistrationOptionsKey(""unitTestDiscovery"")]
    [RegistrationOptionsConverterAttribute(typeof(UnitTestRegistrationOptionsConverter))]
    public partial class UnitTestRegistrationOptions : OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions
    {
        [Optional]
        public bool WorkDoneProgress
        {
            get;
            set;
        }

        class UnitTestRegistrationOptionsConverter : RegistrationOptionsConverterBase<UnitTestRegistrationOptions, StaticOptions>
        {
            public UnitTestRegistrationOptionsConverter()
            {
            }

            public override StaticOptions Convert(UnitTestRegistrationOptions source)
            {
                return new StaticOptions{SupportsDebugging = source.SupportsDebugging, WorkDoneProgress = source.WorkDoneProgress};
            }
        }

        [RegistrationOptionsKey(""unitTestDiscovery"")]
        public partial class StaticOptions : IWorkDoneProgressOptions
        {
            [Optional]
            public bool SupportsDebugging
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
            var expectedStrongTypes = @"
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

#nullable enable
namespace Lsp.Tests.Integration.Fixtures
{
    [Parallel, Method(""tests/run"", Direction.ClientToServer)]
    [
        GenerateHandler,
        GenerateHandlerMethods(typeof(ILanguageServerRegistry)),
        GenerateRequestMethods(typeof(ILanguageClient))
    ]
    [RegistrationOptions(typeof(UnitTestRegistrationOptions)), Capability(typeof(UnitTestCapability))]
    public partial class UnitTest : IRequest
    {
        public string Name { get; set; } = null!;
    }

    [Parallel, Method(""tests/discover"", Direction.ClientToServer)]
    [
        GenerateHandler,
        GenerateHandlerMethods(typeof(ILanguageServerRegistry)),
        GenerateRequestMethods(typeof(ILanguageClient))
    ]
    [RegistrationOptions(typeof(UnitTestRegistrationOptions)), Capability(typeof(UnitTestCapability))]
    public partial class DiscoverUnitTestsParams : IPartialItemsRequest<Container<UnitTest>, UnitTest>, IWorkDoneProgressParams
    {
        public ProgressToken? PartialResultToken { get; set; } = null!;
        public ProgressToken? WorkDoneToken { get; set; } = null!;
    }

    [CapabilityKey(""workspace"", ""unitTests"")]
    public partial class UnitTestCapability : DynamicCapability
    {
        public string Property { get; set; } = null!;
    }

    [GenerateRegistrationOptions(""unitTestDiscovery"")]
    public partial class UnitTestRegistrationOptions : IWorkDoneProgressOptions
    {
        [Optional] public bool SupportsDebugging { get; set; }
    }
}
#nullable restore
";
            var expectedHandlers = @"
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
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Lsp.Tests.Integration.Fixtures
{
    [Parallel, Method(""tests/discover"", Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IDiscoverUnitTestsHandler : IJsonRpcRequestHandler<DiscoverUnitTestsParams, Container<UnitTest>>, IRegistration<UnitTestRegistrationOptions, UnitTestCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class DiscoverUnitTestsHandlerBase : AbstractHandlers.Request<DiscoverUnitTestsParams, Container<UnitTest>, UnitTestRegistrationOptions, UnitTestCapability>, IDiscoverUnitTestsHandler
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class DiscoverUnitTestsPartialHandlerBase : AbstractHandlers.PartialResults<DiscoverUnitTestsParams, Container<UnitTest>, UnitTest, UnitTestRegistrationOptions, UnitTestCapability>, IDiscoverUnitTestsHandler
    {
        protected DiscoverUnitTestsPartialHandlerBase(System.Guid id, IProgressManager progressManager): base(progressManager, Container<UnitTest>.From)
        {
        }
    }
}
#nullable restore

namespace Lsp.Tests.Integration.Fixtures
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class DiscoverUnitTestsExtensions
    {
        public static ILanguageServerRegistry OnDiscoverUnitTests(this ILanguageServerRegistry registry, Func<DiscoverUnitTestsParams, Task<Container<UnitTest>>> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler(""tests/discover"", new LanguageProtocolDelegatingHandlers.Request<DiscoverUnitTestsParams, Container<UnitTest>, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, Container<UnitTest>>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDiscoverUnitTests(this ILanguageServerRegistry registry, Func<DiscoverUnitTestsParams, CancellationToken, Task<Container<UnitTest>>> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler(""tests/discover"", new LanguageProtocolDelegatingHandlers.Request<DiscoverUnitTestsParams, Container<UnitTest>, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, Container<UnitTest>>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDiscoverUnitTests(this ILanguageServerRegistry registry, Func<DiscoverUnitTestsParams, UnitTestCapability, CancellationToken, Task<Container<UnitTest>>> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler(""tests/discover"", new LanguageProtocolDelegatingHandlers.Request<DiscoverUnitTestsParams, Container<UnitTest>, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, Container<UnitTest>>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveDiscoverUnitTests(this ILanguageServerRegistry registry, Action<DiscoverUnitTestsParams, IObserver<IEnumerable<UnitTest>>> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler(""tests/discover"", _ => new LanguageProtocolDelegatingHandlers.PartialResults<DiscoverUnitTestsParams, Container<UnitTest>, UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(PartialAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<UnitTest>.From));
        }

        public static ILanguageServerRegistry ObserveDiscoverUnitTests(this ILanguageServerRegistry registry, Action<DiscoverUnitTestsParams, IObserver<IEnumerable<UnitTest>>, CancellationToken> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler(""tests/discover"", _ => new LanguageProtocolDelegatingHandlers.PartialResults<DiscoverUnitTestsParams, Container<UnitTest>, UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(PartialAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<UnitTest>.From));
        }

        public static ILanguageServerRegistry ObserveDiscoverUnitTests(this ILanguageServerRegistry registry, Action<DiscoverUnitTestsParams, IObserver<IEnumerable<UnitTest>>, UnitTestCapability, CancellationToken> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler(""tests/discover"", _ => new LanguageProtocolDelegatingHandlers.PartialResults<DiscoverUnitTestsParams, Container<UnitTest>, UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(PartialAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<UnitTest>.From));
        }

        public static IRequestProgressObservable<IEnumerable<UnitTest>, Container<UnitTest>> RequestDiscoverUnitTests(this ILanguageClient mediator, DiscoverUnitTestsParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, value => new Container<UnitTest>(value), cancellationToken);
    }
#nullable restore
}";
            await GenerationHelpers.AssertGeneratedAsExpected<RegistrationOptionsGenerator>(source, expectedOptions);
            await GenerationHelpers.AssertGeneratedAsExpected<StronglyTypedGenerator>(source, expectedStrongTypes);
            await GenerationHelpers.AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expectedHandlers);
        }

        [Fact]
        public async Task Supports_Generating_Void_Task_Return()
        {
            var source = @"
// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated a code generator.
// </auto-generated>
// ------------------------------------------------------------------------------

using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

#nullable enable
namespace Lsp.Tests.Integration.Fixtures
{
    [Parallel]
    [Method(ClientNames.RegisterCapability, Direction.ServerToClient)]
    [
        GenerateHandler(""OmniSharp.Extensions.LanguageServer.Protocol.Client"", Name = ""RegisterCapability""),
        GenerateHandlerMethods(typeof(ILanguageClientRegistry),
        GenerateRequestMethods(typeof(IClientLanguageServer), typeof(ILanguageServer))
    ]
    public class RegistrationParams : IJsonRpcRequest
    {
        public RegistrationContainer Registrations { get; set; } = null!;
    }

    /// <summary>
    /// General parameters to to regsiter for a capability.
    /// </summary>
    [DebuggerDisplay(""{"" + nameof(DebuggerDisplay) + "",nq}"")]
    [GenerateContainer]
    public partial class Registration
    {
    }
}
#nullable restore";

            var expectedHandlers = @"
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
}";
            await GenerationHelpers.AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expectedHandlers);
        }
    }
}

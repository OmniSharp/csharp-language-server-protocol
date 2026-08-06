# Migrating from 0.19.x to 1.0

Version 1.0 removes the MediatR dependency and replaces its public request and pipeline contracts with equivalent contracts owned by `OmniSharp.Extensions.JsonRpc`.

## Package references

Remove a direct MediatR package reference if your application used it only for this library. The OmniSharp packages no longer reference or configure MediatR.

## Namespace changes

Replace the MediatR import in handlers, request models, and pipeline behaviors:

```diff
-using MediatR;
+using OmniSharp.Extensions.JsonRpc;
```

The following contract names and behavior remain available from the new namespace:

| 0.19.x | 1.0 |
| --- | --- |
| `MediatR.IRequest<TResponse>` | `OmniSharp.Extensions.JsonRpc.IRequest<TResponse>` |
| `MediatR.IRequest` | `OmniSharp.Extensions.JsonRpc.IRequest` |
| `MediatR.IRequestHandler<TRequest, TResponse>` | `OmniSharp.Extensions.JsonRpc.IRequestHandler<TRequest, TResponse>` |
| `MediatR.IRequestHandler<TRequest>` | `OmniSharp.Extensions.JsonRpc.IRequestHandler<TRequest>` |
| `MediatR.IPipelineBehavior<TRequest, TResponse>` | `OmniSharp.Extensions.JsonRpc.IPipelineBehavior<TRequest, TResponse>` |
| `MediatR.RequestHandlerDelegate<TResponse>` | `OmniSharp.Extensions.JsonRpc.RequestHandlerDelegate<TResponse>` |
| `MediatR.Unit` | `OmniSharp.Extensions.JsonRpc.Unit` |

## Handlers

Generated LSP and DAP handler interfaces now inherit the JSON-RPC-owned handler contracts. Existing handler method signatures remain the same after changing the namespace import.

```csharp
using OmniSharp.Extensions.JsonRpc;

public sealed class HoverHandler : IHoverHandler
{
    public Task<Hover?> Handle(HoverParams request, CancellationToken cancellationToken)
    {
        return Task.FromResult<Hover?>(null);
    }
}
```

Code that implemented a handler through an explicitly qualified MediatR interface must instead implement the generated handler interface or the corresponding `OmniSharp.Extensions.JsonRpc.IRequestHandler<,>` interface.

## Pipeline behaviors

Change custom pipeline behavior registrations and implementations to the JSON-RPC-owned interface. Open-generic dependency injection registrations keep the same shape:

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

Pipeline behavior ordering, cancellation-token forwarding, and nested `next` delegate execution are unchanged.

## Unit

Notification and no-result request handlers now return `OmniSharp.Extensions.JsonRpc.Unit`. If a file also uses `System.Reactive.Unit`, add aliases to make the intended type explicit:

```csharp
using JsonRpcUnit = OmniSharp.Extensions.JsonRpc.Unit;
using ReactiveUnit = System.Reactive.Unit;
```

## Runtime behavior

Incoming requests are dispatched directly to the handler selected by the JSON-RPC descriptor. Request scopes, pipeline behavior composition, cancellation, notification fan-out, aggregate responses, and exception mapping retain their 0.19.x behavior.
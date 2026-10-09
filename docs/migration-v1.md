# Migrating from 0.19.x to 1.0

Version 1.0 removes the MediatR and Newtonsoft.Json dependencies. It replaces the public request and pipeline contracts with equivalent contracts owned by `OmniSharp.Extensions.JsonRpc` and uses System.Text.Json for arbitrary JSON values.

## Package references

Remove a direct MediatR package reference if your application used it only for this library. The OmniSharp packages no longer reference or configure MediatR.

Likewise, remove a direct Newtonsoft.Json package reference if it was needed only for OmniSharp protocol models. Keep it if other application code still uses Newtonsoft.Json.

## JSON serialization

Protocol models now use System.Text.Json instead of Newtonsoft.Json. Add the appropriate imports when working with arbitrary JSON values:

```diff
-using Newtonsoft.Json.Linq;
+using System.Text.Json;
+using System.Text.Json.Nodes;
```

The affected public types are:

| 0.19.x | 1.0 |
| --- | --- |
| `LSPAny.Value: JToken?` | `LSPAny.Value: JsonElement` |
| `LSPObject : JObject` | `LSPObject`, wrapping a `JsonObject` in `Value` |
| `LSPArray : JArray` | `LSPArray`, wrapping a `JsonArray` in `Value` |

### Reading `LSPAny`

Use `JsonElement` APIs such as `ValueKind`, `TryGetProperty`, `GetProperty`, `EnumerateArray`, and the typed `Get*` methods:

```diff
-JToken value = request.Data.Value;
-string? name = (string?)value["name"];
-foreach (JToken item in value["items"] ?? new JArray())
+JsonElement value = request.Data.Value;
+string? name = value.TryGetProperty("name", out var nameProperty)
+    ? nameProperty.GetString()
+    : null;
+
+if (value.TryGetProperty("items", out var items))
 {
-    Console.WriteLine((string?)item);
+    foreach (JsonElement item in items.EnumerateArray())
+    {
+        Console.WriteLine(item.GetString());
+    }
 }
```

`LSPAny` clones incoming `JsonElement` values, so its `Value` does not depend on the lifetime of the `JsonDocument` from which the element originated. Create arbitrary protocol values with `LSPAny.From`:

```csharp
var data = LSPAny.From(new
{
    name = "example",
    enabled = true
});
```

### Building objects and arrays

`LSPObject` and `LSPArray` are no longer subclasses of the Newtonsoft LINQ-to-JSON types. Construct them directly or work with their mutable `Value` properties:

```csharp
var payload = new LSPObject
{
    ["name"] = "example",
    ["items"] = new LSPArray(1, "two", false)
};

JsonObject jsonObject = payload.Value;
JsonArray jsonArray = new LSPArray(1, 2, 3).Value;
LSPAny data = LSPAny.From(payload);
```

`LSPObject` retains a string indexer, but it returns `JsonNode?` rather than `JToken?`. `LSPArray` does not inherit collection APIs; use its `Value` property for indexing, enumeration, and mutation:

```csharp
var values = new LSPArray("first", "second");
values.Value[0] = "updated";

foreach (JsonNode? value in values.Value)
{
    Console.WriteLine(value?.ToJsonString());
}
```

Both wrappers have a one-way implicit conversion to `JsonNode`. That conversion returns a deep clone, so mutating the converted node does not mutate the wrapper:

```csharp
JsonNode clonedNode = payload;
```

There is no implicit conversion from `JsonNode`, `JsonObject`, or `JsonArray` back to an LSP wrapper. Use `LSPAny.From(node)` when an arbitrary protocol value is sufficient, or populate a new `LSPObject`/`LSPArray` when the wrapper type is required.

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
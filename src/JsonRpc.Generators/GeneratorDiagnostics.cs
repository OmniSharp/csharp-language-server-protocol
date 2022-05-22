using Microsoft.CodeAnalysis;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    // TODO: Create analyier to ensure registration options are { get; set; }
    // TODO: Create analyier to ensure capabilities are { get; set; }
    internal static class GeneratorDiagnostics
    {
        public static DiagnosticDescriptor Exception { get; } = new DiagnosticDescriptor(
            "JRPC0001", "Exception",
            "{0} - {1}", "JRPC", DiagnosticSeverity.Error, true
        );

        public static DiagnosticDescriptor NoHandlerRegistryProvided { get; } = new DiagnosticDescriptor(
            "JRPC1000", "No Handler Registry Provided",
            "No Handler Registry Provided for handler {0}.", "JsonRPC", DiagnosticSeverity.Info, true
        );

        public static DiagnosticDescriptor NoResponseRouterProvided { get; } = new DiagnosticDescriptor(
            "JRPC1001", "No Response Router Provided",
            "No Response Router Provided for handler {0}, defaulting to {1}.", "JsonRPC", DiagnosticSeverity.Info, true
        );

        public static DiagnosticDescriptor MissingDirection { get; } = new DiagnosticDescriptor(
            "JRPC1002", "Missing Direction",
            "No direction defined on request Debug Adapter Protocol or Language Server Protocol for inference to work correctly, please specify the target interface(s).", "JsonRPC", DiagnosticSeverity.Error, true
        );

        public static DiagnosticDescriptor CouldNotInferRequestRouter { get; } = new DiagnosticDescriptor(
            "JRPC1003", "Cannot infer request router(s)",
            "Could not infer the request router(s) to use, please specify the target interface(s).", "JsonRPC", DiagnosticSeverity.Error, true
        );

        public static DiagnosticDescriptor MustBePartial { get; } = new DiagnosticDescriptor(
            "JRPC1004", "Type must be made partial",
            "Type {0} must be made partial.", "JsonRPC", DiagnosticSeverity.Error, true
        );

        public static DiagnosticDescriptor MustBeReadOnly { get; } = new DiagnosticDescriptor(
            "JRPC1005", "Type must be made readonly",
            "Type {0} must be made readonly.", "JsonRPC", DiagnosticSeverity.Error, true
        );

        public static DiagnosticDescriptor MustBeARequestOrNotification { get; } = new DiagnosticDescriptor(
            "JRPC1006", "Type must implement a request or notification interface",
            "Type {0} must implement a request or notification interface.", "JsonRPC", DiagnosticSeverity.Error, true
        );

        public static DiagnosticDescriptor MustInheritFromCanBeResolved { get; } = new DiagnosticDescriptor(
            "LSP1001", "The target class must implement ICanBeResolved",
            "The target class must implement ICanBeResolved", "LSP", DiagnosticSeverity.Error, true
        );

        public static DiagnosticDescriptor MustInheritFromCanHaveData { get; } = new DiagnosticDescriptor(
            "LSP1002", "The target class must implement ICanHaveData",
            "The target class must implement ICanHaveData", "LSP", DiagnosticSeverity.Error, true
        );
    }
}

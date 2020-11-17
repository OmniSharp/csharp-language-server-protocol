using Microsoft.CodeAnalysis;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    internal static class GeneratorDiagnostics
    {
        public static DiagnosticDescriptor MissingDirection { get; } = new DiagnosticDescriptor(
            "LSP1000", "Missing Direction",
            "No direction defined for Language Server Protocol Handler", "LSP", DiagnosticSeverity.Warning, true
        );
        public static DiagnosticDescriptor Exception { get; } = new DiagnosticDescriptor(
            "JRPC0001", "Exception",
            "{0}", "JRPC", DiagnosticSeverity.Error, true
        );

        public static DiagnosticDescriptor NoHandlerRegistryProvided { get; } = new DiagnosticDescriptor(
            "JRPC1000", "No Handler Registry Provided",
            "No Handler Registry Provided for handler {0}.", "JsonRPC", DiagnosticSeverity.Info, true
        );

        public static DiagnosticDescriptor NoResponseRouterProvided { get; } = new DiagnosticDescriptor(
            "JRPC1001", "No Response Router Provided",
            "No Response Router Provided for handler {0}, defaulting to {1}.", "JsonRPC", DiagnosticSeverity.Info, true
        );

        public static DiagnosticDescriptor ClassMustBePartial { get; } = new DiagnosticDescriptor(
            "JRPC1002", "Class must be made partial",
            "Class {0} must be made partial.", "JsonRPC", DiagnosticSeverity.Warning, true
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

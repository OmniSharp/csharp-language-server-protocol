using Microsoft.CodeAnalysis;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    internal static class GeneratorDiagnostics
    {
        public static DiagnosticDescriptor MissingDirection { get; } = new DiagnosticDescriptor(
            "LSP1000", "Missing Direction",
            "No direction defined for Language Server Protocol Handler", "JsonRPC", DiagnosticSeverity.Warning, true
        );

        public static DiagnosticDescriptor Message { get; } = new DiagnosticDescriptor(
            "LSP9999", "DEBUG",
            "{0}", "JsonRPC", DiagnosticSeverity.Error, true
        );

        public static DiagnosticDescriptor NoHandlerRegistryProvided { get; } = new DiagnosticDescriptor(
            "JRPC1000", "No Handler Registry Provided",
            "No Handler Registry Provided for handler {0}.", "JsonRPC", DiagnosticSeverity.Warning, true
        );

        public static DiagnosticDescriptor NoResponseRouterProvided { get; } = new DiagnosticDescriptor(
            "JRPC1001", "No Response Router Provided",
            "No Response Router Provided for handler {0}, defaulting to {1}.", "JsonRPC", DiagnosticSeverity.Warning, true
        );
    }
}

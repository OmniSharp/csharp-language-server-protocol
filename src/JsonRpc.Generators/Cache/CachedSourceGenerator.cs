using System;
using Microsoft.CodeAnalysis;

namespace OmniSharp.Extensions.JsonRpc.Generators.Cache
{
    /// <summary>
    /// We're not supposed to do this... but in realistic.
    /// </summary>
    public abstract class CachedSourceGenerator<T, TSyntax> : ISourceGenerator
        where T : ISyntaxReceiver, IReceiverCache<TSyntax>
        where TSyntax : SyntaxNode
    {
        private readonly Func<T> _syntaxReceiverFactory;

        public CachedSourceGenerator(Func<T> syntaxReceiverFactory)
        {
            _syntaxReceiverFactory = syntaxReceiverFactory;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => _syntaxReceiverFactory());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!( context.SyntaxReceiver is T syntaxReceiver )) return;

            syntaxReceiver.Start(context);
            Execute(
                context, syntaxReceiver,
                (name, node, text) => {
                    context.AddSource(name, text);

                    if (CacheKeyHasher.Cache)
                    {
                        syntaxReceiver.AddCacheSource(name, node, text);
                    }
                },
                (node, diagnostic) => {
                    context.ReportDiagnostic(diagnostic(node));

                    if (CacheKeyHasher.Cache)
                    {
                        syntaxReceiver.ReportCacheDiagnostic(node, diagnostic);
                    }
                }
            );
            foreach (var item in syntaxReceiver.CachedSources)
            {
                context.AddSource(item.Name, item.SourceText);
            }

            foreach (var item in syntaxReceiver.CachedDiagnostics)
            {
                context.ReportDiagnostic(item);
            }

            syntaxReceiver.Finish(context);
        }

        protected abstract void Execute(
            GeneratorExecutionContext context, T syntaxReceiver, AddCacheSource<TSyntax> addCacheSource, ReportCacheDiagnostic<TSyntax> cacheDiagnostic
        );
    }
}

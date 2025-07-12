using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    internal interface IExtensionMethodGeneratorStrategy
    {
        IEnumerable<MemberDeclarationSyntax> Apply(SourceProductionContext context, GeneratorData item);
    }
}

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    internal interface IExtensionMethodContextGeneratorStrategy
    {
        IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, GeneratorData item);
    }
}

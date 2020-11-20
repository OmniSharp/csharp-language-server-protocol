using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    internal interface ICompilationUnitGeneratorStrategy
    {
        IEnumerable<MemberDeclarationSyntax> Apply(GeneratorData item);
    }
}

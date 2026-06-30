using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class TypeHierarchyTests
    {
        [Fact]
        public void Subtypes_Typed_Conversions_Preserve_Distinct_Progress_Tokens()
        {
            var parameters = new TypeHierarchySubtypesParams
            {
                Item = new TypeHierarchyItem
                {
                    Name = "C",
                    Kind = SymbolKind.Class,
                    Uri = "file:///workspace/c.cs",
                    Range = new Range(new Position(1, 2), new Position(3, 4)),
                    SelectionRange = new Range(new Position(1, 2), new Position(1, 3))
                },
                PartialResultToken = "partial",
                WorkDoneToken = "work"
            };

            var typed = TypeHierarchySubtypesParams.Create<TestHandlerIdentity>(parameters);
            var untyped = TypeHierarchySubtypesParams<TestHandlerIdentity>.Create(typed);

            typed.PartialResultToken.Should().Be(parameters.PartialResultToken);
            typed.WorkDoneToken.Should().Be(parameters.WorkDoneToken);
            untyped.PartialResultToken.Should().Be(parameters.PartialResultToken);
            untyped.WorkDoneToken.Should().Be(parameters.WorkDoneToken);
        }

        private class TestHandlerIdentity : IHandlerIdentity
        {
            public string __identity { get; init; } = string.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class WorkspaceEditTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new WorkspaceEdit() {
                Changes = new Dictionary<Uri, IEnumerable<TextEdit>>() {
                        {
                            new Uri("file:///abc/123/d.cs"), new [] {
                                new TextEdit() {
                                    NewText = "new text",
                                    Range = new Range(new Position(1, 1), new Position(2,2))
                                },
                                new TextEdit() {
                                    NewText = "new text2",
                                    Range = new Range(new Position(3, 3), new Position(4,4))
                                }
                            }
                        }
                    }
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<WorkspaceEdit>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

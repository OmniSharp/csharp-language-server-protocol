using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CodeLensTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CodeLens() {
                Command = new Command() {
                    Arguments = new JArray { 1, "2", true },
                    Name = "abc",
                    Title = "Cool story bro"
                },
                Data = new Dictionary<string, object>()
                {
                    { "somethingCool" , 1 }
                },
                Range = new Range(new Position(1, 2), new Position(2, 3)),
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            // TODO: Come back and fix this...
            //var deresult = JsonConvert.DeserializeObject<CodeLens>(expected);
            //deresult.ShouldBeEquivalentTo(model);
        }
    }
}

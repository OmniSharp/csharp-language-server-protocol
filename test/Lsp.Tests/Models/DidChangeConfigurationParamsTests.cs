using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DidChangeConfigurationParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DidChangeConfigurationParams() {
                Settings = new Dictionary<string, BooleanNumberString>() {
                    { "abc", 1 },
                    { "def", "a" },
                    { "ghi", true },
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DidChangeConfigurationParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

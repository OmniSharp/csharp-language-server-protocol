using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class RegistrationTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Registration() {
                Id = "abc",
                Method = "method",
                RegisterOptions = new Dictionary<string, object>()
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<Registration>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

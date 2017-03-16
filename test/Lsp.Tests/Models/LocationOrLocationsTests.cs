using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class LocationOrLocationsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new LocationOrLocations();
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<LocationOrLocations>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

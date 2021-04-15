using System;
using FluentAssertions;
using OmniSharp.Extensions.DebugAdapter.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using Xunit;

namespace Dap.Tests
{
    public class EnumLikeConverterTests
    {
        [Fact]
        public void PathFormat_Should_Be_Serializable()
        {
            var options = new InitializeRequestArguments() {
                PathFormat = PathFormat.Uri
            };

            Action a = () => new DapSerializer().SerializeObject(options);
            a.Should().NotThrow();
        }
        [Fact]
        public void PathFormat_Should_Be_Deserializable()
        {
            Action a = () => new DapSerializer().DeserializeObject<InitializeRequestArguments>("{\"pathformat\": \"Uri\"}");
            a.Should().NotThrow();
        }
    }
}

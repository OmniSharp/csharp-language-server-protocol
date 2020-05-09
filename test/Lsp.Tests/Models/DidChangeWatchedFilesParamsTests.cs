using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DidChangeWatchedFilesParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DidChangeWatchedFilesParams() {
                Changes = new[] {
                    new FileEvent() {
                        Type = FileChangeType.Created,
                        Uri = new Uri("file:///someawesomefile")
                    }
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DidChangeWatchedFilesParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void NonStandardCharactersTest(string expected)
        {
            var model = new DidChangeWatchedFilesParams() {
                Changes = new[] {
                    new FileEvent() {
                        Type = FileChangeType.Created,
                        // Mörkö
                        Uri = new Uri("file:///Mörkö.cs")
                    }
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DidChangeWatchedFilesParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}

using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;
using Xunit;

namespace Lsp.Tests.Models
{
    public class ShowMessageRequestParamsTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ShowMessageRequestParams {
                Message = "message",
                Actions = new Container<MessageActionItem>(
                    new MessageActionItem {
                        Title = "abc"
                    }
                ),
                Type = MessageType.Error
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<ShowMessageRequestParams>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }

        [Fact]
        public void Should_RoundTrip_Extension_Data()
        {
            var model = new MessageActionItem {
                Title = "abc",
                ExtensionData = new Dictionary<string, JsonElement> {
                    ["custom"] = JsonSerializer.SerializeToElement(new { enabled = true })
                }
            };

            var result = new LspSerializer(ClientVersion.Lsp3).SerializeObject(model);
            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<MessageActionItem>(result);

            deresult.Title.Should().Be(model.Title);
            deresult.ExtensionData["custom"].GetRawText().Should().Be(model.ExtensionData["custom"].GetRawText());
        }
    }
}

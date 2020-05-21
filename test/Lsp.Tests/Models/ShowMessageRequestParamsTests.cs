using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class ShowMessageRequestParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ShowMessageRequestParams() {
                Message = "message",
                Actions = new Container<MessageActionItem>(new MessageActionItem() {
                    Title = "abc"
                }),
                Type = MessageType.Error
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<ShowMessageRequestParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}

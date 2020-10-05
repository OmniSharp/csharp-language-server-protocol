using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class LanguageServerLoggingTests : LanguageProtocolTestBase
    {
        public LanguageServerLoggingTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions())
        {
        }

        private readonly List<LogMessageParams> _logs = new List<LogMessageParams>();

        [Fact]
        public async Task Logs_Are_Sent_To_Client_From_Server()
        {
            var (client, server) = await Initialize(
                options => {  },
                options => {
                    options.ConfigureLogging(
                        z => z
                            .AddLanguageProtocolLogging(LogLevel.Trace)
                            .SetMinimumLevel(LogLevel.Trace)
                    );
                }
            );

            await SettleNext();

            using var _ = client.Register(r => r.OnLogMessage(x => { _logs.Add(x); }));

            var logger = server.GetRequiredService<ILogger<ILanguageServer>>();

            logger.LogCritical("holy cow!");
            logger.LogError("Something bad happened...");
            logger.LogInformation("Here's something cool...");
            logger.LogWarning("Uh-oh...");
            logger.LogTrace("Just gotta let you trace!");
            logger.LogDebug("Just gotta let you debug!");

            await _logs.DelayUntilCount(6, CancellationToken);

            _logs.Should().HaveCount(6);
            _logs.Where(z => z.Type == MessageType.Error).Should().HaveCount(2);
            _logs.Where(z => z.Type == MessageType.Info).Should().HaveCount(1);
            _logs.Where(z => z.Type == MessageType.Warning).Should().HaveCount(1);
            _logs.Where(z => z.Type == MessageType.Log).Should().HaveCount(2);
        }

        [Fact]
        public async Task Logs_Are_Sent_To_Client_From_Server_Respecting_SetMinimumLevel()
        {
            var (client, server) = await Initialize(
                options => { },
                options => {
                    options.ConfigureLogging(
                        z => z
                            .AddLanguageProtocolLogging(LogLevel.Trace)
                            .SetMinimumLevel(LogLevel.Warning)
                    );
                }
            );

            await SettleNext();

            using var _ = client.Register(r => r.OnLogMessage(x => { _logs.Add(x); }));

            var logger = server.GetRequiredService<ILogger<ILanguageServer>>();

            logger.LogCritical("holy cow!");
            logger.LogError("Something bad happened...");
            logger.LogInformation("Here's something cool...");
            logger.LogWarning("Uh-oh...");
            logger.LogTrace("Just gotta let you trace!");
            logger.LogDebug("Just gotta let you debug!");

            await _logs.DelayUntilCount(3, CancellationToken);

            _logs.Should().HaveCount(3);
            _logs.Where(z => z.Type == MessageType.Error).Should().HaveCount(2);
            _logs.Where(z => z.Type == MessageType.Info).Should().HaveCount(0);
            _logs.Where(z => z.Type == MessageType.Warning).Should().HaveCount(1);
            _logs.Where(z => z.Type == MessageType.Log).Should().HaveCount(0);
        }

        [Fact]
        public async Task Logs_Are_Sent_To_Client_From_Server_Respecting_TraceLevel()
        {
            var (client, server) = await Initialize(
                options => {  },
                options => {
                    options.ConfigureLogging(
                        z => z
                            .AddLanguageProtocolLogging(LogLevel.Information)
                            .SetMinimumLevel(LogLevel.Trace)
                    );
                }
            );

            await SettleNext();

            using var _ = client.Register(r => r.OnLogMessage(x => { _logs.Add(x); }));

            var logger = server.GetRequiredService<ILogger<ILanguageServer>>();

            logger.LogCritical("holy cow!");
            logger.LogError("Something bad happened...");
            logger.LogInformation("Here's something cool...");
            logger.LogWarning("Uh-oh...");
            logger.LogTrace("Just gotta let you trace!");
            logger.LogDebug("Just gotta let you debug!");

            await _logs.DelayUntilCount(4, CancellationToken);

            _logs.Should().HaveCount(4);
            _logs.Where(z => z.Type == MessageType.Error).Should().HaveCount(2);
            _logs.Where(z => z.Type == MessageType.Info).Should().HaveCount(1);
            _logs.Where(z => z.Type == MessageType.Warning).Should().HaveCount(1);
            _logs.Where(z => z.Type == MessageType.Log).Should().HaveCount(0);
        }
    }
}

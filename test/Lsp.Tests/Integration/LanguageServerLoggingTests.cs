using System;
using System.Collections.Concurrent;
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
        public LanguageServerLoggingTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().WithCancellationTimeout(TimeSpan.FromMinutes(2)))
        {
        }

        [Fact]//[RetryFact]
        public async Task Logs_Are_Sent_To_Client_From_Server()
        {
            var logs = new ConcurrentBag<LogMessageParams>();
            var (client, server) = await Initialize(
                options => { options.Trace = InitializeTrace.Verbose; },
                options => {
                    options.ConfigureLogging(
                        z => z
                            .AddLanguageProtocolLogging()
                            .SetMinimumLevel(LogLevel.Trace)
                    );
                }
            );

            await SettleNext();

            using var _ = client.Register(r => r.OnLogMessage(x => { logs.Add(x); }));

            var logger = server.GetRequiredService<ILogger<ILanguageServer>>();

            logger.LogCritical("holy cow!");
            logger.LogError("Something bad happened...");
            logger.LogInformation("Here's something cool...");
            logger.LogWarning("Uh-oh...");
            logger.LogTrace("Just gotta let you trace!");
            logger.LogDebug("Just gotta let you debug!");

            await logs.DelayUntilCount(6, CancellationToken);
            var items = logs.Take(6).ToList();

            items.Should().HaveCount(6);
            items.Where(z => z.Type == MessageType.Error).Should().HaveCount(2);
            items.Where(z => z.Type == MessageType.Info).Should().HaveCount(1);
            items.Where(z => z.Type == MessageType.Warning).Should().HaveCount(1);
            items.Where(z => z.Type == MessageType.Log).Should().HaveCount(2);
        }

        [Fact]//[RetryFact]
        public async Task Logs_Are_Sent_To_Client_From_Server_Respecting_SetMinimumLevel()
        {
            var logs = new ConcurrentBag<LogMessageParams>();
            var (client, server) = await Initialize(
                options => { },
                options => {
                    options.ConfigureLogging(
                        z => z
                            .AddLanguageProtocolLogging()
                            .SetMinimumLevel(LogLevel.Warning)
                    );
                }
            );

            await SettleNext();

            using var _ = client.Register(r => r.OnLogMessage(x => { logs.Add(x); }));

            var logger = server.GetRequiredService<ILogger<ILanguageServer>>();

            logger.LogCritical("holy cow!");
            logger.LogError("Something bad happened...");
            logger.LogInformation("Here's something cool...");
            logger.LogWarning("Uh-oh...");
            logger.LogTrace("Just gotta let you trace!");
            logger.LogDebug("Just gotta let you debug!");

            await logs.DelayUntilCount(3, CancellationToken);
            var items = logs.Take(3).ToList();

            items.Should().HaveCount(3);
            items.Where(z => z.Type == MessageType.Error).Should().HaveCount(2);
            items.Where(z => z.Type == MessageType.Info).Should().HaveCount(0);
            items.Where(z => z.Type == MessageType.Warning).Should().HaveCount(1);
            items.Where(z => z.Type == MessageType.Log).Should().HaveCount(0);
        }

        [Fact]//[RetryFact]
        public async Task Logs_Are_Sent_To_Client_From_Server_Respecting_TraceLevel()
        {
            var logs = new ConcurrentBag<LogMessageParams>();
            var (client, server) = await Initialize(
                options => { options.Trace = InitializeTrace.Messages; },
                options => {
                    options.ConfigureLogging(
                        z => z
                            .AddLanguageProtocolLogging()
                            .SetMinimumLevel(LogLevel.Trace)
                    );
                }
            );

            await SettleNext();

            using var _ = client.Register(r => r.OnLogMessage(x => { logs.Add(x); }));

            var logger = server.GetRequiredService<ILogger<ILanguageServer>>();

            logger.LogCritical("holy cow!");
            logger.LogError("Something bad happened...");
            logger.LogInformation("Here's something cool...");
            logger.LogWarning("Uh-oh...");
            logger.LogTrace("Just gotta let you trace!");
            logger.LogDebug("Just gotta let you debug!");

            await logs.DelayUntilCount(4, CancellationToken);
            var items = logs.Take(4).ToList();

            items.Should().HaveCount(4);
            items.Where(z => z.Type == MessageType.Error).Should().HaveCount(2);
            items.Where(z => z.Type == MessageType.Info).Should().HaveCount(1);
            items.Where(z => z.Type == MessageType.Warning).Should().HaveCount(1);
            items.Where(z => z.Type == MessageType.Log).Should().HaveCount(0);
        }

        [Fact]//[RetryFact]
        public async Task Client_Can_Dynamically_Change_Server_Trace_Level_Off_To_Verbose()
        {
            var logs = new ConcurrentBag<LogMessageParams>();
            var (client, server) = await Initialize(
                options => { },
                options => {
                    options.ConfigureLogging(
                        z => z
                            .AddLanguageProtocolLogging()
                            .SetMinimumLevel(LogLevel.Trace)
                    );
                }
            );

            await SettleNext();

            using var _ = client.Register(r => r.OnLogMessage(x => { logs.Add(x); }));

            var logger = server.GetRequiredService<ILogger<ILanguageServer>>();

            logger.LogCritical("holy cow!");
            logger.LogError("Something bad happened...");
            logger.LogInformation("Here's something cool...");
            logger.LogWarning("Uh-oh...");
            logger.LogTrace("Just gotta let you trace!");
            logger.LogDebug("Just gotta let you debug!");

            await logs.DelayUntilCount(3, CancellationToken);
            {
                var items = logs.Take(3).ToList();
                ;

                items.Should().HaveCount(3);
                items.Where(z => z.Type == MessageType.Error).Should().HaveCount(2);
                items.Where(z => z.Type == MessageType.Info).Should().HaveCount(0);
                items.Where(z => z.Type == MessageType.Warning).Should().HaveCount(1);
                items.Where(z => z.Type == MessageType.Log).Should().HaveCount(0);
            }

            client.SetTrace(new() { Value = InitializeTrace.Verbose });
            await SettleNext();

            logs.Clear();

            logger.LogCritical("holy cow!");
            logger.LogError("Something bad happened...");
            logger.LogInformation("Here's something cool...");
            logger.LogWarning("Uh-oh...");
            logger.LogTrace("Just gotta let you trace!");
            logger.LogDebug("Just gotta let you debug!");

            await logs.DelayUntilCount(6, CancellationToken);
            {
                var items = logs.Take(6).ToList();
                ;

                items.Should().HaveCount(6);
                items.Where(z => z.Type == MessageType.Error).Should().HaveCount(2);
                items.Where(z => z.Type == MessageType.Info).Should().HaveCount(1);
                items.Where(z => z.Type == MessageType.Warning).Should().HaveCount(1);
                items.Where(z => z.Type == MessageType.Log).Should().HaveCount(2);
            }
        }

        [Fact]//[RetryFact]
        public async Task Client_Can_Dynamically_Change_Server_Trace_Level_Verbose_To_Off()
        {
            var logs = new ConcurrentBag<LogMessageParams>();
            var (client, server) = await Initialize(
                options => {
                    options.Trace = InitializeTrace.Verbose;
                },
                options => {
                    options.ConfigureLogging(
                        z => z
                            .AddLanguageProtocolLogging()
                            .SetMinimumLevel(LogLevel.Trace)
                    );
                }
            );

            await SettleNext();

            using var _ = client.Register(r => r.OnLogMessage(x => { logs.Add(x); }));

            var logger = server.GetRequiredService<ILogger<ILanguageServer>>();

            logger.LogCritical("holy cow!");
            logger.LogError("Something bad happened...");
            logger.LogInformation("Here's something cool...");
            logger.LogWarning("Uh-oh...");
            logger.LogTrace("Just gotta let you trace!");
            logger.LogDebug("Just gotta let you debug!");

            await logs.DelayUntilCount(6, CancellationToken);
            {
                var items = logs.Take(6).ToList();

                items.Should().HaveCount(6);
                items.Where(z => z.Type == MessageType.Error).Should().HaveCount(2);
                items.Where(z => z.Type == MessageType.Info).Should().HaveCount(1);
                items.Where(z => z.Type == MessageType.Warning).Should().HaveCount(1);
                items.Where(z => z.Type == MessageType.Log).Should().HaveCount(2);
            }

            client.SetTrace(new() { Value = InitializeTrace.Off });
            await SettleNext();

            logs.Clear();

            logger.LogCritical("holy cow!");
            logger.LogError("Something bad happened...");
            logger.LogInformation("Here's something cool...");
            logger.LogWarning("Uh-oh...");
            logger.LogTrace("Just gotta let you trace!");
            logger.LogDebug("Just gotta let you debug!");

            await logs.DelayUntilCount(3, CancellationToken);
            {
                var items = logs.Take(3).ToList();

                items.Should().HaveCount(3);
                items.Where(z => z.Type == MessageType.Error).Should().HaveCount(2);
                items.Where(z => z.Type == MessageType.Info).Should().HaveCount(0);
                items.Where(z => z.Type == MessageType.Warning).Should().HaveCount(1);
                items.Where(z => z.Type == MessageType.Log).Should().HaveCount(0);
            }
        }
    }
}

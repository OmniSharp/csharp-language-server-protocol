using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Execute command registration options.
    /// </summary>
    public class ExecuteCommandRegistrationOptions : WorkDoneProgressOptions, IRegistrationOptions
    {
        /// <summary>
        /// The commands to be executed on the server
        /// </summary>
        public Container<string> Commands { get; set; }

        /// <summary>
        /// Execute command options.
        /// </summary>
        public class StaticOptions : WorkDoneProgressOptions
        {
            /// <summary>
            /// The commands to be executed on the server
            /// </summary>
            public Container<string> Commands { get; set; }
        }

        class ExecuteCommandRegistrationOptionsConverter : RegistrationOptionsConverterBase<ExecuteCommandRegistrationOptions, StaticOptions>
        {
            private readonly IHandlersManager _handlersManager;

            public ExecuteCommandRegistrationOptionsConverter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.ExecuteCommandProvider))
            {
                _handlersManager = handlersManager;
            }
            public override StaticOptions Convert(ExecuteCommandRegistrationOptions source)
            {
                var allRegistrationOptions = _handlersManager.Descriptors
                                                             .OfType<ILspHandlerDescriptor>()
                                                             .Where(z => z.HasRegistration)
                                                             .Select(z => z.RegistrationOptions)
                                                             .OfType<ExecuteCommandRegistrationOptions>()
                                                             .ToArray();
                return new StaticOptions {
                    Commands = allRegistrationOptions
                              .SelectMany(z => z.Commands)
                              .ToArray(),
                    WorkDoneProgress = allRegistrationOptions.Any(x => x.WorkDoneProgress == true)
                };
            }
        }
    }
}

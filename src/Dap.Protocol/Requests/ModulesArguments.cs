using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class ModulesArguments : IRequest<ModulesResponse>
    {
        /// <summary>
        /// The index of the first module to return; if omitted modules start at 0.
        /// </summary>
        [Optional] public long? StartModule { get; set; }

        /// <summary>
        /// The number of modules to return. If moduleCount is not specified or 0, all modules are returned.
        /// </summary>
        [Optional] public long? ModuleCount { get; set; }
    }

}

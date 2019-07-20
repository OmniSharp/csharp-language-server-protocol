using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ModulesResponse
    {
        /// <summary>
        /// All modules or range of modules.
        /// </summary>
        public Container<Module> modules { get; set; }

        /// <summary>
        /// The total number of modules available.
        /// </summary>
        [Optional] public long? totalModules { get; set; }
    }

}

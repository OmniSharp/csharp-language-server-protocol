using System.Collections.Generic;
using Newtonsoft.Json;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// A Module object represents a row in the modules view.
    /// Two attributes are mandatory: an id identifies a module in the modules view and is used in a ModuleEvent for identifying a module for adding, updating or deleting.
    /// The name is used to minimally render the module in the UI.
    /// Additional attributes can be added to the module.They will show up in the module View if they have a corresponding ColumnDescriptor.
    /// To avoid an unnecessary proliferation of additional attributes with similar semantics but different names
    /// we recommend to re-use attributes from the ‘recommended’ list below first, and only introduce new attributes if nothing appropriate could be found.
    /// </summary>

    public class Module
    {
        /// <summary>
        /// Unique identifier for the module.
        /// </summary>
        public NumberString Id { get; set; }

        /// <summary>
        /// A name of the module.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// optional but recommended attributes.
        /// always try to use these first before introducing additional attributes.
        ///
        /// Logical full path to the module. The exact definition is implementation defined, but usually this would be a full path to the on-disk file for the module.
        /// </summary>
        [Optional] public string Path { get; set; }

        /// <summary>
        /// True if the module is optimized.
        /// </summary>
        [Optional] public bool? IsOptimized { get; set; }

        /// <summary>
        /// True if the module is considered 'user code' by a debugger that supports 'Just My Code'.
        /// </summary>
        [Optional] public bool? IsUserCode { get; set; }

        /// <summary>
        /// Version of Module.
        /// </summary>
        [Optional] public string Version { get; set; }

        /// <summary>
        /// User understandable description of if symbols were found for the module (ex: 'Symbols Loaded', 'Symbols not found', etc.
        /// </summary>
        [Optional] public string SymbolStatus { get; set; }

        /// <summary>
        /// Logical full path to the symbol file. The exact definition is implementation defined.
        /// </summary>
        [Optional] public string SymbolFilePath { get; set; }

        /// <summary>
        /// Module created or modified.
        /// </summary>
        [Optional] public string DateTimeStamp { get; set; }

        /// <summary>
        /// Address range covered by this module.
        /// </summary>
        [Optional] public string AddressRange { get; set; }

        /// <summary>
        /// Allows additional data to be displayed
        /// </summary>
        [JsonExtensionData] public Dictionary<string, object> ExtensionData { get; set; } = new Dictionary<string, object>();
    }
}

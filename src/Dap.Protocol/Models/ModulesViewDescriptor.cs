namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// The ModulesViewDescriptor is the container for all declarative configuration options of a ModuleView.
    /// For now it only specifies the columns to be shown in the modules view.
    /// </summary>
    public class ModulesViewDescriptor
    {
        public Container<ColumnDescriptor> Columns { get; set; }
    }
}

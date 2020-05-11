using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>VariablePresentationHint
    /// Optional properties of a variable that can be used to determine how to render the variable in the UI.
    /// </summary>
    public class VariablePresentationHint
    {
        /// <summary>
        /// The kind of variable. Before introducing additional values, try to use the listed values.
        /// Values:
        /// 'property': Indicates that the object is a property.
        /// 'method': Indicates that the object is a method.
        /// 'class': Indicates that the object is a class.
        /// 'data': Indicates that the object is data.
        /// 'event': Indicates that the object is an event.
        /// 'baseClass': Indicates that the object is a base class.
        /// 'innerClass': Indicates that the object is an inner class.
        /// 'interface': Indicates that the object is an interface.
        /// 'mostDerivedClass': Indicates that the object is the most derived class.
        /// 'virtual': Indicates that the object is virtual, that means it is a synthetic object introduced by the adapter for rendering purposes, e.g. an index range for large arrays.
        /// 'dataBreakpoint': Indicates that a data breakpoint is registered for the object.
        /// etc.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string Kind { get; set; }

        /// <summary>
        /// Set of attributes represented as an array of strings. Before introducing additional values, try to use the listed values.
        /// Values:
        /// 'static': Indicates that the object is static.
        /// 'constant': Indicates that the object is a constant.
        /// 'readOnly': Indicates that the object is read only.
        /// 'rawString': Indicates that the object is a raw string.
        /// 'hasObjectId': Indicates that the object can have an Object ID created for it.
        /// 'canHaveObjectId': Indicates that the object has an Object ID associated with it.
        /// 'hasSideEffects': Indicates that the evaluation had side effects.
        /// etc.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public Container<string> Attributes { get; set; }

        /// <summary>
        /// Visibility of variable. Before introducing additional values, try to use the listed values.
        /// Values: 'public', 'private', 'protected', 'internal', 'final', etc.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string Visibility { get; set; }
    }
}

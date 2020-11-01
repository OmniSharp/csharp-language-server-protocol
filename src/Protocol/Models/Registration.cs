using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// General paramters to to regsiter for a capability.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Registration
    {
        /// <summary>
        /// The id used to register the request. The id can be used to deregister
        /// the request again.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// The method / capability to register for.
        /// </summary>
        public string Method { get; set; } = null!;

        /// <summary>
        /// Options necessary for the registration.
        /// </summary>
        [Optional]
        public object? RegisterOptions { get; set; }

        private string DebuggerDisplay => $"[{Id}] {( RegisterOptions is ITextDocumentRegistrationOptions td ? $"{td.DocumentSelector}" : string.Empty )} {Method}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;

        public class TextDocumentComparer : IEqualityComparer<Registration?>
        {
            public bool Equals(Registration? x, Registration? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                if (x.RegisterOptions is ITextDocumentRegistrationOptions xTdro && y.RegisterOptions is ITextDocumentRegistrationOptions yTdro)
                {
                    // Id doesn't matter if they target the same text document
                    // this is arbitrary but should be good in most cases.
                    return ( x.Id == y.Id || xTdro.DocumentSelector == yTdro.DocumentSelector ) && x.Method == y.Method;
                }

                return x.Id == y.Id && x.Method == y.Method;
            }

            public int GetHashCode(Registration? obj)
            {
                unchecked
                {
                    if (obj!.RegisterOptions is ITextDocumentRegistrationOptions tdro)
                    {
                        var hashCode = obj.Method.GetHashCode();
                        return ( hashCode * 397 ) ^ ( tdro.DocumentSelector != null ? tdro.DocumentSelector.GetHashCode() : 0 );
                    }
                    else
                    {
                        var hashCode = obj!.Id.GetHashCode();
                        hashCode = ( hashCode * 397 ) ^ obj.Method.GetHashCode();
                        return ( hashCode * 397 ) ^ ( obj.RegisterOptions != null ? obj.RegisterOptions.GetHashCode() : 0 );
                    }
                }
            }
        }
    }
}

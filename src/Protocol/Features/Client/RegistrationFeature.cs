using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(ClientNames.RegisterCapability, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Client", Name = "RegisterCapability")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IClientLanguageServer), typeof(ILanguageServer))]
        public partial record RegistrationParams : IJsonRpcRequest
        {
            public RegistrationContainer Registrations { get; init; } = null!;
        }

        [Parallel]
        [Method(ClientNames.UnregisterCapability, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Client", Name = "UnregisterCapability")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IClientLanguageServer), typeof(ILanguageServer))]
        public partial record UnregistrationParams : IJsonRpcRequest
        {
            public UnregistrationContainer? Unregisterations { get; init; }

            // Placeholder for v4 support
            [JsonIgnore]
            public UnregistrationContainer? Unregistrations
            {
                get => Unregisterations;
                init => Unregisterations = value;
            }
        }

        /// <summary>
        /// General paramters to to regsiter for a capability.
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [GenerateContainer]
        public partial record Registration
        {
            /// <summary>
            /// The id used to register the request. The id can be used to deregister
            /// the request again.
            /// </summary>
            public string Id { get; init; } = null!;

            /// <summary>
            /// The method / capability to register for.
            /// </summary>
            public string Method { get; init; } = null!;

            /// <summary>
            /// Options necessary for the registration.
            /// </summary>
            [Optional]
            public object? RegisterOptions { get; init; }

            private string DebuggerDisplay =>
                $"[{Id}] {( RegisterOptions is ITextDocumentRegistrationOptions td ? $"{td.DocumentSelector}" : string.Empty )} {Method}";

            /// <inheritdoc />
            public override string ToString()
            {
                return DebuggerDisplay;
            }

            public static Unregistration ToUnregistration(Registration registration)
            {
                return new Unregistration
                {
                    Id = registration.Id,
                    Method = registration.Method,
                };
            }

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
                        return ( x.Id == y.Id || xTdro.DocumentSelector! == yTdro.DocumentSelector! ) && x.Method == y.Method;
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
                            return ( hashCode * 397 ) ^ ( tdro.DocumentSelector?.GetHashCode() ?? 0 );
                        }
                        else
                        {
                            var hashCode = obj!.Id.GetHashCode();
                            hashCode = ( hashCode * 397 ) ^ obj.Method.GetHashCode();
                            return ( hashCode * 397 ) ^ ( obj.RegisterOptions?.GetHashCode() ?? 0 );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// General parameters to unregister a request or notification.
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [GenerateContainer]
        public partial record Unregistration
        {
            /// <summary>
            /// The id used to unregister the request or notification. Usually an id
            /// provided during the register request.
            /// </summary>
            public string Id { get; init; } = null!;

            /// <summary>
            /// The method to unregister for.
            /// </summary>
            public string Method { get; init; } = null!;

            public static implicit operator Unregistration(Registration registration)
            {
                return new Unregistration
                {
                    Id = registration.Id,
                    Method = registration.Method
                };
            }

            private string DebuggerDisplay => $"[{Id}] {Method}";

            /// <inheritdoc />
            public override string ToString()
            {
                return DebuggerDisplay;
            }
        }

        public partial class UnregistrationContainer
        {
            [return: NotNullIfNotNull("items")]
            public static UnregistrationContainer? From(IEnumerable<Registration>? items)
            {
                return items switch
                {
                    not null => new UnregistrationContainer(items.Select(Registration.ToUnregistration)),
                    _        => null
                };
            }

            [return: NotNullIfNotNull("items")]
            public static implicit operator UnregistrationContainer?(Registration[] items)
            {
                return items switch
                {
                    not null => new UnregistrationContainer(items.Select(Registration.ToUnregistration)),
                    _        => null
                };
            }

            [return: NotNullIfNotNull("items")]
            public static UnregistrationContainer? From(params Registration[] items)
            {
                return items switch
                {
                    not null => new UnregistrationContainer(items.Select(Registration.ToUnregistration)),
                    _        => null
                };
            }

            [return: NotNullIfNotNull("items")]
            public static implicit operator UnregistrationContainer?(Collection<Registration>? items)
            {
                return items switch
                {
                    not null => new UnregistrationContainer(items.Select(Registration.ToUnregistration)),
                    _        => null
                };
            }

            [return: NotNullIfNotNull("items")]
            public static UnregistrationContainer? From(Collection<Registration>? items)
            {
                return items switch
                {
                    not null => new UnregistrationContainer(items.Select(Registration.ToUnregistration)),
                    _        => null
                };
            }

            [return: NotNullIfNotNull("items")]
            public static implicit operator UnregistrationContainer?(List<Registration>? items)
            {
                return items switch
                {
                    not null => new UnregistrationContainer(items.Select(Registration.ToUnregistration)),
                    _        => null
                };
            }

            [return: NotNullIfNotNull("items")]
            public static UnregistrationContainer? From(List<Registration>? items)
            {
                return items switch
                {
                    not null => new UnregistrationContainer(items.Select(Registration.ToUnregistration)),
                    _        => null
                };
            }

            [return: NotNullIfNotNull("items")]
            public static implicit operator UnregistrationContainer?(ImmutableList<Registration>? items)
            {
                return items switch
                {
                    not null => new UnregistrationContainer(items.Select(Registration.ToUnregistration)),
                    _        => null
                };
            }

            [return: NotNullIfNotNull("items")]
            public static UnregistrationContainer? From(ImmutableList<Registration>? items)
            {
                return items switch
                {
                    not null => new UnregistrationContainer(items.Select(Registration.ToUnregistration)),
                    _        => null
                };
            }
        }
    }

    namespace Client
    {
    }
}

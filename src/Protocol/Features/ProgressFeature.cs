using System.Diagnostics;
using System.Reflection;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Serial]
        [Method(GeneralNames.Progress, Direction.Bidirectional)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IGeneralLanguageClient), typeof(ILanguageClient), typeof(IGeneralLanguageServer), typeof(ILanguageServer))]
        public record ProgressParams : IRequest<Unit>
        {
            public static ProgressParams Create<T>(ProgressToken token, T value, JsonSerializer jsonSerializer)
            {
                return new ProgressParams
                {
                    Token = token,
                    Value = JToken.FromObject(value, jsonSerializer)
                };
            }

            /// <summary>
            /// The progress token provided by the client or server.
            /// </summary>
            public ProgressToken Token { get; init; } = null!;

            /// <summary>
            /// The progress data.
            /// </summary>
            public JToken Value { get; init; } = null!;
        }

        [JsonConverter(typeof(ProgressTokenConverter))]
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public record ProgressToken : IEquatable<long>, IEquatable<string>
        {
            private long? _long;
            private string? _string;

            public ProgressToken(long value)
            {
                _long = value;
                _string = null;
            }

            public ProgressToken(string value)
            {
                _long = null;
                _string = value;
            }

            public bool IsLong => _long.HasValue;

            public long Long
            {
                get => _long ?? 0;
                set
                {
                    String = null;
                    _long = value;
                }
            }

            public bool IsString => _string != null;

            public string? String
            {
                get => _string;
                set
                {
                    _string = value;
                    _long = null;
                }
            }

            public static implicit operator ProgressToken(long value)
            {
                return new ProgressToken(value);
            }

            public static implicit operator ProgressToken(string value)
            {
                return new ProgressToken(value);
            }

            public ProgressParams Create<T>(T value, JsonSerializer jsonSerializer)
            {
                return ProgressParams.Create(this, value, jsonSerializer);
            }

            public bool Equals(long other)
            {
                return IsLong && Long == other;
            }

            public bool Equals(string other)
            {
                return IsString && String == other;
            }

            private string DebuggerDisplay => IsString ? String! : IsLong ? Long.ToString() : "";

            /// <inheritdoc />
            public override string ToString()
            {
                return DebuggerDisplay;
            }
        }
    }

    public static partial class ProgressExtensions
    {
        public static IRequestProgressObservable<TItem, TResponse> RequestProgress<TResponse, TItem>(
            this ILanguageProtocolProxy requestRouter,
            IPartialItemRequest<TResponse, TItem> @params,
            Func<TItem, TResponse> factory,
            Func<TResponse, TItem> reverseFactory,
            CancellationToken cancellationToken = default
        ) where TResponse : TItem
        {
            @params.SetPartialResultToken(new ProgressToken(Guid.NewGuid().ToString()));

            return requestRouter.ProgressManager.MonitorUntil(@params, factory, reverseFactory, cancellationToken);
        }

        public static IRequestProgressObservable<IEnumerable<TItem>, TResponse> RequestProgress<TResponse, TItem>(
            this ILanguageProtocolProxy requestRouter, IPartialItemsRequest<TResponse, TItem> @params, Func<IEnumerable<TItem>, TResponse> factory,
            CancellationToken cancellationToken = default
        )
            where TResponse : IEnumerable<TItem>
        {
            @params.SetPartialResultToken(new ProgressToken(Guid.NewGuid().ToString()));

            return requestRouter.ProgressManager.MonitorUntil(@params, factory, cancellationToken);
        }

        private static readonly PropertyInfo PartialResultTokenProperty =
            typeof(IPartialResultParams).GetProperty(nameof(IPartialResultParams.PartialResultToken))!;

        internal static ProgressToken SetPartialResultToken(this IPartialResultParams @params, ProgressToken? progressToken = null)
        {
            if (@params.PartialResultToken is not null) return @params.PartialResultToken;
            PartialResultTokenProperty.SetValue(@params, progressToken ?? new ProgressToken(Guid.NewGuid().ToString()));
            return @params.PartialResultToken!;
        }
    }
}

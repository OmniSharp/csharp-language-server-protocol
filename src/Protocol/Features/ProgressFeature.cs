using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
        [Parallel]
        [Method(GeneralNames.Progress, Direction.Bidirectional)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol"), GenerateHandlerMethods,
         GenerateRequestMethods(typeof(IGeneralLanguageClient), typeof(ILanguageClient), typeof(IGeneralLanguageServer), typeof(ILanguageServer))]
        public class ProgressParams : IRequest
        {
            public static ProgressParams Create<T>(ProgressToken token, T value, JsonSerializer jsonSerializer) =>
                new ProgressParams {
                    Token = token,
                    Value = JToken.FromObject(value, jsonSerializer)
                };

            /// <summary>
            /// The progress token provided by the client or server.
            /// </summary>
            public ProgressToken Token { get; set; } = null!;

            /// <summary>
            /// The progress data.
            /// </summary>
            public JToken Value { get; set; } = null!;
        }

        [JsonConverter(typeof(ProgressTokenConverter))]
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public class ProgressToken : IEquatable<ProgressToken>, IEquatable<long>, IEquatable<string>
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
                set {
                    String = null;
                    _long = value;
                }
            }

            public bool IsString => _string != null;

            public string? String
            {
                get => _string;
                set {
                    _string = value;
                    _long = null;
                }
            }

            public static implicit operator ProgressToken(long value) => new ProgressToken(value);

            public static implicit operator ProgressToken(string value) => new ProgressToken(value);

            public ProgressParams Create<T>(T value, JsonSerializer jsonSerializer) => ProgressParams.Create(this, value, jsonSerializer);

            public override bool Equals(object obj) =>
                obj is ProgressToken token &&
                Equals(token);

            public override int GetHashCode()
            {
                var hashCode = 1456509845;
                hashCode = hashCode * -1521134295 + IsLong.GetHashCode();
                hashCode = hashCode * -1521134295 + Long.GetHashCode();
                hashCode = hashCode * -1521134295 + IsString.GetHashCode();
                if (String != null) hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(String);
                return hashCode;
            }

            public bool Equals(ProgressToken other) =>
                IsLong == other.IsLong &&
                Long == other.Long &&
                IsString == other.IsString &&
                String == other.String;

            public bool Equals(long other) => IsLong && Long == other;

            public bool Equals(string other) => IsString && String == other;

            private string DebuggerDisplay => IsString ? String! : IsLong ? Long.ToString() : "";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }
    }

    public static partial class ProgressExtensions
    {
        public static IRequestProgressObservable<TItem, TResponse> RequestProgress<TResponse, TItem>(
            this ILanguageProtocolProxy requestRouter, IPartialItemRequest<TResponse, TItem> @params, Func<TItem, TResponse> factory, CancellationToken cancellationToken = default
        )
        {
            var resultToken = new ProgressToken(Guid.NewGuid().ToString());
            @params.PartialResultToken = resultToken;

            return requestRouter.ProgressManager.MonitorUntil(@params, factory, cancellationToken);
        }

        public static IRequestProgressObservable<IEnumerable<TItem>, TResponse> RequestProgress<TResponse, TItem>(
            this ILanguageProtocolProxy requestRouter, IPartialItemsRequest<TResponse, TItem> @params, Func<IEnumerable<TItem>, TResponse> factory,
            CancellationToken cancellationToken = default
        )
            where TResponse : IEnumerable<TItem>
        {
            var resultToken = new ProgressToken(Guid.NewGuid().ToString());
            @params.PartialResultToken = resultToken;

            return requestRouter.ProgressManager.MonitorUntil(@params, factory, cancellationToken);
        }
    }
}

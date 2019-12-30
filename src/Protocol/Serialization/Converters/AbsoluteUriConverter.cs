// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 	// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// #see https://github.com/NuGet/NuGet.Server
using System;
using System.Text;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    /// <summary>
    /// This is necessary because Newtonsoft.Json creates <see cref="Uri"/> instances with
    /// <see cref="UriKind.RelativeOrAbsolute"/> which treats UNC paths as relative. NuGet.Core uses
    /// <see cref="UriKind.Absolute"/> which treats UNC paths as absolute. For more details, see:
    /// https://github.com/JamesNK/Newtonsoft.Json/issues/2128
    /// </summary>
    class AbsoluteUriConverter : JsonConverter<Uri>
    {
        public override Uri ReadJson(JsonReader reader, Type objectType, Uri existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                var uri = new Uri((string)reader.Value, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri)
                {
                    throw new JsonSerializationException($"The Uri must be absolute. Given: {reader.Value}");
                }
                return uri;
            }

            throw new JsonSerializationException("The JSON value must be a string.");
        }

        public override void WriteJson(JsonWriter writer, Uri value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            if (!value.IsAbsoluteUri)
            {
                throw new JsonSerializationException("The URI value must be an absolute Uri. Relative URI instances are not allowed.");
            }

            if (value.IsFile)
            {
                // First add the file scheme and ://
                var builder = new StringBuilder(value.Scheme)
                    .Append("://");

                // UNC file paths use the Host
                if (value.HostNameType != UriHostNameType.Basic)
                {
                    builder.Append(value.Host);
                }

                // Paths that start with a drive letter don't have a slash in the PathAndQuery
                // but they need it in the final result.
                if (value.PathAndQuery[0] != '/')
                {
                    builder.Append('/');
                }

                // Lastly add the remaining parts of the URL
                builder.Append(value.PathAndQuery);
                writer.WriteValue(builder.ToString());
            }
            else
            {
                writer.WriteValue(value.AbsoluteUri);
            }
        }
    }
}

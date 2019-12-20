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

            if (!(value is Uri uriValue))
            {
                throw new JsonSerializationException("The value must be a URI.");
            }

            writer.WriteValue(Convert(uriValue));
        }

        public static string Convert(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                throw new JsonSerializationException("The URI value must be an absolute Uri. Relative URI instances are not allowed.");
            }

            if (uri.IsFile)
            {
                // First add the file scheme and ://
                var builder = new StringBuilder(uri.Scheme)
                    .Append("://");

                // UNC file paths use the Host
                if (uri.HostNameType != UriHostNameType.Basic)
                {
                    builder.Append(uri.Host);
                }

                // Paths that start with a drive letter don't have a slash in the PathAndQuery
                // but they need it in the final result.
                if (uri.PathAndQuery[0] != '/')
                {
                    builder.Append('/');
                }

                // Lastly add the remaining parts of the URL
                builder.Append(uri.PathAndQuery);
                return builder.ToString();
            }

            return uri.AbsoluteUri;
        }
    }
}

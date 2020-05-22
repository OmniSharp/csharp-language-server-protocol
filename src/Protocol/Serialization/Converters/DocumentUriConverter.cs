// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 	// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// #see https://github.com/NuGet/NuGet.Server

using System;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class DocumentUriConverter : JsonConverter<DocumentUri>
    {
        public override DocumentUri ReadJson(JsonReader reader, Type objectType, DocumentUri existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.TokenType == JsonToken.String)
            {
                try
                {
                    return DocumentUri.Parse((string) reader.Value);
                }
                catch (ArgumentException ex)
                {
                    throw new JsonSerializationException("Could not deserialize document uri", ex);
                }
            }

            throw new JsonSerializationException("The JSON value must be a string.");
        }

        public override void WriteJson(JsonWriter writer, DocumentUri value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value.ToString());
        }
    }
}

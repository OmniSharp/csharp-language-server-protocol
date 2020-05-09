// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 	// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// #see https://github.com/NuGet/NuGet.Server

using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class DocumentUriConverter : JsonConverter<DocumentUri>
    {
        public override DocumentUri Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        public override void Write(Utf8JsonWriter writer, DocumentUri value, JsonSerializerOptions options)
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

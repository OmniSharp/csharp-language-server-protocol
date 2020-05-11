using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(Converter))]
    public struct LocationOrLocationLink {

        public LocationOrLocationLink(Location location)
        {
            Location = location;
            LocationLink = null;
        }

        public LocationOrLocationLink(LocationLink locationLink)
        {
            Location = null;
            LocationLink = locationLink;
        }

        public bool IsLocation => Location != null;
        public Location Location { get; }

        public bool IsLocationLink => LocationLink != null;
        public LocationLink LocationLink { get; }

        public static implicit operator LocationOrLocationLink(Location location)
        {
            return new LocationOrLocationLink(location);
        }

        public static implicit operator LocationOrLocationLink(LocationLink locationLink)
        {
            return new LocationOrLocationLink(locationLink);
        }

    class Converter : JsonConverter<LocationOrLocationLink>
    {
        public override void Write(Utf8JsonWriter writer, LocationOrLocationLink value, JsonSerializerOptions options)
        {
            if (value.IsLocation)
                  JsonSerializer.Serialize(writer, value.Location, options);
            if (value.IsLocationLink)
                  JsonSerializer.Serialize(writer, value.LocationLink, options);
        }

        public override LocationOrLocationLink Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected an object");

            // TODO: Is there a better way to do this?
            // Perhaps a way to peek into the reader and see if there is a property name, and come back to start here?
            DocumentUri uri = null;
            Range range= null;
            Range originSelectionRange= null;
            DocumentUri targetUri= null;
            Range targetRange= null;
            Range targetSelectionRange= null;
            string propertyName = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); break;}
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    continue;
                }

                switch (propertyName)
                {
                    case nameof(uri):
                        uri = new DocumentUri(reader.GetString());
                        break;

                    case nameof(range):
                        range = JsonSerializer.Deserialize<Range>(ref reader, options);
                        break;
                    case nameof(originSelectionRange):
                        originSelectionRange = JsonSerializer.Deserialize<Range>(ref reader, options);
                        break;
                    case nameof(targetUri):
                        targetUri = new DocumentUri(reader.GetString());
                        break;
                    case nameof(targetRange):
                        targetRange = JsonSerializer.Deserialize<Range>(ref reader, options);
                        break;
                    case nameof(targetSelectionRange):
                        targetSelectionRange = JsonSerializer.Deserialize<Range>(ref reader, options);
                        break;
                    default:
                        throw new JsonException($"Unsupported property found {propertyName}");
                }
            }

            if (uri != null)
            {
                return new Location() {
                    Range = range,
                    Uri = uri
                };
            }

            return new LocationLink() {
                TargetRange = targetRange,
                TargetUri = targetUri,
                OriginSelectionRange = originSelectionRange,
                TargetSelectionRange = targetSelectionRange
            };
        }
    }
    }
}

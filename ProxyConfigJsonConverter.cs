using System.Text.Json;
using System.Text.Json.Serialization;

namespace hkrpg.proxy
{
    public class ProxyConfigJsonConverter : JsonConverter<ProxyConfig>
    {
        public override ProxyConfig Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected start of object");
            }

            var config = new ProxyConfig
            {
                DestinationHost = "127.0.0.1",  // Default value
                DestinationPort = 8080,         // Default value
                ProxyBindPort = 8080            // Default value
            };
            
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return config;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Expected property name");
                }

                string propertyName = reader.GetString() ?? throw new JsonException("Property name cannot be null");
                reader.Read();

                switch (propertyName)
                {
                    case "DestinationHost":
                        config.DestinationHost = reader.GetString() ?? throw new JsonException("DestinationHost cannot be null");
                        break;
                    case "DestinationPort":
                        config.DestinationPort = reader.GetInt32();
                        break;
                    case "ProxyBindPort":
                        config.ProxyBindPort = reader.GetInt32();
                        break;
                    case "LastGamePath":
                        config.LastGamePath = reader.GetString();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            throw new JsonException("Expected end of object");
        }

        public override void Write(Utf8JsonWriter writer, ProxyConfig value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            
            writer.WriteString("DestinationHost", value.DestinationHost);
            writer.WriteNumber("DestinationPort", value.DestinationPort);
            writer.WriteNumber("ProxyBindPort", value.ProxyBindPort);
            
            if (value.LastGamePath != null)
            {
                writer.WriteString("LastGamePath", value.LastGamePath);
            }
            
            writer.WriteEndObject();
        }
    }
} 
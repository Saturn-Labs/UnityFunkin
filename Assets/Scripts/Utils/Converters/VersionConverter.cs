using System;
using Newtonsoft.Json;
using Version = SemanticVersioning.Version;

namespace Utils.Converters
{
    public class VersionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is Version version)
            {
                writer.WriteValue(version.ToString());
            }
            else
            {
                writer.WriteValue("1.0.0");
            }
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value is not string versionString)
                return null;
            if (Version.TryParse(versionString, out var version))
                return version;
            throw new JsonSerializationException($"Invalid version string: {versionString}");
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(Version);
    }
}
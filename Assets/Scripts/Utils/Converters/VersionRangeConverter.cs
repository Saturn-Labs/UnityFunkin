using System;
using Newtonsoft.Json;
using Range = SemanticVersioning.Range;

namespace Utils.Converters
{
    public class VersionRangeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is Range range)
            {
                writer.WriteValue(range.ToString());
            }
            else
            {
                writer.WriteValue("*");
            }
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value is not string rangeString)
                return null;
            if (string.IsNullOrEmpty(rangeString))
                return null;
            if (Range.TryParse(rangeString, out var range))
                return range;
            throw new JsonSerializationException($"Invalid range string: {rangeString}");
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(Range);
    }
}
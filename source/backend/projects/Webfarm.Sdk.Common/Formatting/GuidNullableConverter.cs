namespace Webfarm.Sdk.Common.Formatting
{
    using System;
    using Newtonsoft.Json;

    public class GuidNullableConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Guid?) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Guid? result;
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    result = null;
                    break;
                case JsonToken.String:
                    var str = reader.Value as string;
                    result = string.IsNullOrEmpty(str) ? (Guid?)null : new Guid(str);
                    break;
                default:
                    throw new ArgumentException("Invalid token type");
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var guid = value as Guid?;
            var s = guid?.ToString("N");
            writer.WriteValue(s);
        }
    }
}

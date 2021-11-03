namespace Webfarm.Sdk.Common.Formatting
{
    using System;
    using Newtonsoft.Json;

    public class GuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Guid) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Guid result;
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    result = Guid.Empty;
                    break;
                case JsonToken.String:
                    var str = reader.Value as string;
                    result = string.IsNullOrEmpty(str) ? Guid.Empty : new Guid(str);
                    break;
                default:
                    throw new ArgumentException("Invalid token type");
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var s = Guid.Empty.Equals(value) ? string.Empty : ((Guid)value).ToString("N");
            writer.WriteValue(s);
        }
    }
}

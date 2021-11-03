namespace Webfarm.Sdk.Common.Extensions
{
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Webfarm.Sdk.Common.Formatting;

    public static class SerializationExtensions
    {
        private static readonly JsonSerializerSettings DefaultJsonSerializerSettings =
            new JsonSerializerSettings().ApplyDefaults();

        private static readonly JsonSerializerSettings IndentedJsonSerializerSettings =
            new JsonSerializerSettings().ApplyDefaults(Formatting.Indented);

        [NotNull]
        public static JsonSerializerSettings ApplyDefaults(this JsonSerializerSettings settings)
        {
            #if DEBUG
            return settings.ApplyDefaults(Formatting.Indented);
            #else
            return settings.ApplyDefaults(Formatting.None);
            #endif
        }

        [NotNull]
        public static JsonSerializerSettings ApplyDefaults(this JsonSerializerSettings settings, Formatting formatting)
        {
            settings ??= new JsonSerializerSettings();

            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            settings.Converters.Add(new GuidConverter());
            settings.Converters.Add(new GuidNullableConverter());
            settings.Formatting = formatting;

            return settings;
        }

        /*
        [NotNull]
        public static JsonSerializerOptions ApplyDefaults(this JsonSerializerOptions options)
        {
            options ??= new JsonSerializerOptions();

            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.IgnoreNullValues = true;
            options.Converters.Add(new JsonStringEnumConverter());

            #if DEBUG
            options.WriteIndented = true;
            #else
            options.WriteIndented = false;
            #endif

            return options;
        }
        */

        [NotNull]
        public static string Serialize<T>(this T value)
        {
            var json = JsonConvert.SerializeObject(value, DefaultJsonSerializerSettings);
            return json;
        }

        [CanBeNull]
        public static T Deserialize<T>([CanBeNull] this string json)
        {
            var value = json != null ? JsonConvert.DeserializeObject<T>(json, DefaultJsonSerializerSettings) : default;
            return value;
        }

        public static string ToIndentedJsonString(this object value)
        {
            var json = JsonConvert.SerializeObject(value, IndentedJsonSerializerSettings);
            return json;
        }
    }
}

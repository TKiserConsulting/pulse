namespace Webfarm.Sdk.Common.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using JetBrains.Annotations;
    using Serilog.Events;
    using Serilog.Formatting;
    using Serilog.Formatting.Json;
    using Serilog.Parsing;

    public class SpecializedJsonFormatter : ITextFormatter
    {
        private readonly bool suppressMessageTemplate;
        private readonly bool suppressMessageTemplateParams;
        private readonly JsonValueFormatter valueFormatter;

        public SpecializedJsonFormatter([CanBeNull] JsonValueFormatter valueFormatter = null, bool suppressMessageTemplate = false, bool suppressMessageTemplateParams = true)
        {
            this.valueFormatter = valueFormatter ?? new JsonValueFormatter("$type");
            this.suppressMessageTemplate = suppressMessageTemplate;
            this.suppressMessageTemplateParams = suppressMessageTemplateParams;
        }

        public void Format([NotNull] LogEvent logEvent, [NotNull] TextWriter output)
        {
            this.FormatCore(logEvent, output);
            output.WriteLine();
        }

        [NotNull]
        private static IEnumerable<(string, LogEventPropertyValue)> GetProperties([NotNull] LogEvent logEvent, [CanBeNull] string sourceContext)
        {
            IEnumerable<(string, LogEventPropertyValue)> properties = null;
            if ((sourceContext ?? string.Empty).StartsWith("Microsoft.AspNetCore", StringComparison.OrdinalIgnoreCase) ||
                (sourceContext ?? string.Empty).StartsWith("System.Net.Http.HttpClient", StringComparison.OrdinalIgnoreCase) ||
                (sourceContext ?? string.Empty).StartsWith("Serilog.AspNetCore", StringComparison.OrdinalIgnoreCase))
            {
                properties = GetAspNetCoreProperties(logEvent);
            }

            // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
            properties = properties ?? GetDefaultProperties(logEvent);
            return properties.Where((k, _) =>
            {
                var key = k.Item1;
                var accept = !string.IsNullOrEmpty(key) && key switch
                {
                    "Application" => false,
                    "SourceContext" => false,
                    _ => true
                };

                return accept;
            });
        }

        [NotNull]
        private static IEnumerable<(string, LogEventPropertyValue)> GetDefaultProperties([NotNull] LogEvent logEvent, [CanBeNull] Func<string, string> map = null)
        {
            map ??= s => s;
            return
                from property in logEvent.Properties
                let key = map(property.Key)
                where !string.IsNullOrEmpty(key)
                select (key, property.Value);
        }

        [NotNull]
        private static IEnumerable<(string, LogEventPropertyValue)> GetAspNetCoreProperties([NotNull] LogEvent logEvent)
        {
            return GetDefaultProperties(logEvent, key =>
            {
                return key switch
                {
                    "Protocol" => "server_protocol",
                    "RequestMethod" => "request_method",
                    "Method" => "request_method",
                    "RequestPath" => "request_path",
                    "QueryString" => "request_query",
                    "StatusCode" => "http_status",
                    "Elapsed" => "request_time",
                    "ElapsedMilliseconds" => "request_time",

                    "Host" => null,
                    "Scheme" => null,
                    "PathBase" => null,
                    "Path" => null,
                    "EventId" => null,
                    "HostingRequestFinishedLog" => null,
                    "HostingRequestStartingLog" => null,
                    _ => key
                };
            });
        }

        private void FormatCore(
            [NotNull] LogEvent logEvent,
            [NotNull] TextWriter output)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            string applicationName = null;
            string sourceContext = null;

            LogEventPropertyValue applicationNamePropertyValue;
            if (logEvent.Properties.TryGetValue("Application", out applicationNamePropertyValue))
            {
                applicationName =
                    (applicationNamePropertyValue as ScalarValue)?.Value as string ??
                    applicationNamePropertyValue.ToString();
            }

            LogEventPropertyValue sourceContextPropertyValue;
            if (logEvent.Properties.TryGetValue("SourceContext", out sourceContextPropertyValue))
            {
                sourceContext =
                    (sourceContextPropertyValue as ScalarValue)?.Value as string ??
                    sourceContextPropertyValue.ToString();
            }

            output.Write("{\"@t\":\"");
            output.Write(logEvent.Timestamp.UtcDateTime.ToString("O", CultureInfo.InvariantCulture));
            output.Write('"');

            if (logEvent.Level != LogEventLevel.Information)
            {
                output.Write(",\"@l\":\"");
                output.Write(logEvent.Level);
                output.Write('"');
            }

            output.Write(",\"@m\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Render(logEvent.Properties), output);

            if (logEvent.Exception != null)
            {
                output.Write(",\"@x\":");
                JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
            }

            // do we really need it in the log - it will be the same for all logs from that process
            if (!string.IsNullOrEmpty(applicationName))
            {
                output.Write(",\"@a\":\"");
                output.Write(applicationName);
                output.Write('"');
            }

            // do we really need it in the log - how to use it while log analyzing
            if (!string.IsNullOrEmpty(sourceContext))
            {
                output.Write(",\"@s\":\"");
                output.Write(sourceContext);
                output.Write('"');
            }

            if (!this.suppressMessageTemplate)
            {
                output.Write(",\"@mt\":");
                JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Text, output);

                var source = logEvent.MessageTemplate.Tokens
                    .OfType<PropertyToken>()
                    .Where(pt => pt.Format != null)
                    .ToList();
                if (!this.suppressMessageTemplateParams && source.Any())
                {
                    output.Write(",\"@r\":[");
                    var str = string.Empty;
                    foreach (var propertyToken in source)
                    {
                        output.Write(str);
                        str = ",";
                        using (var stringWriter = new StringWriter())
                        {
                            var properties = logEvent.Properties;
                            propertyToken.Render(properties, stringWriter);
                            JsonValueFormatter.WriteQuotedJsonString(stringWriter.ToString(), output);
                        }
                    }

                    output.Write(']');
                }
            }

            foreach (var (key, propertyValue) in GetProperties(logEvent, sourceContext))
            {
                var str = key;
                if (str.Length > 0 && str[0] == '@')
                {
                    str = "@" + str;
                }

                output.Write(',');
                JsonValueFormatter.WriteQuotedJsonString(str, output);
                output.Write(':');
                this.valueFormatter.Format(propertyValue, output);
            }

            output.Write('}');
        }
    }
}

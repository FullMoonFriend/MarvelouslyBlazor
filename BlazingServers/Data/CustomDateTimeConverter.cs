using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazingServers.Data
{
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:sszzz"; // Adjust format as needed

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            string dateTimeString = reader.GetString();
            // handle System.Text.Json.JsonException: 'Unable to convert "-0001-11-30T00:00:00-0500" to DateTime.'
            if (dateTimeString == "-0001-11-30T00:00:00-0500")
            {
                return DateTime.MinValue;
            }

            if (DateTime.TryParseExact(dateTimeString, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }

            throw new JsonException($"Unable to convert \"{dateTimeString}\" to DateTime.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateTimeFormat));
        }
    }
}

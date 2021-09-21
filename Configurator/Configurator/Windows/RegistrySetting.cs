using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Configurator.Windows
{
    public class RegistrySetting
    {
        public string KeyName { get; set; }
        public string ValueName { get; set; }
        [JsonConverter(typeof(RegistrySettingValueDataConverter))]
        public object ValueData { get; set; }
    }

    internal class RegistrySettingValueDataConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Number => GetNumber(reader),
                _ => throw new Exception($"{nameof(RegistrySetting)}.{nameof(RegistrySetting.ValueData)} only supports types: string, int32, and double")
            })!;
        }

        private static object GetNumber(Utf8JsonReader reader)
        {
            return reader.TryGetInt32(out var @int)
                ? (object)@int
                : reader.GetDouble();
        }

        public override void Write(Utf8JsonWriter writer, object obj, JsonSerializerOptions options)
        {
            throw new NotSupportedException($"There is no need yet for serializing {nameof(RegistrySetting)}");
        }
    }
}

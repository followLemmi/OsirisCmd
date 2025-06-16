using System.Text.Json;
using System.Text.Json.Serialization;

namespace OsirisCmd.SettingsManager.Converters;

public class SettingsSectionConverter : JsonConverter<ISettingsSection>
{
    public override ISettingsSection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, ISettingsSection value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
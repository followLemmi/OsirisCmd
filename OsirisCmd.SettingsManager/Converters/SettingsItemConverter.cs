using System.Text.Json;
using System.Text.Json.Serialization;

namespace OsirisCmd.SettingsManager.Converters;

public class SettingsItemConverter : JsonConverter<SettingItem>
{
    public override SettingItem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        var name = root.GetProperty("Name").GetString();
        var typeString = root.GetProperty("Type").GetString();
        var type = Type.GetType(typeString);
        var valueElement = root.GetProperty("Value");
        
        if (type == null) 
            throw new Exception($"Type {typeString} not found.");
        
        var value = valueElement.Deserialize(type, options);
        return new SettingItem()
        {
            Name = name!,
            Type = typeString,
            Value = value!,
        };
    }

    public override void Write(Utf8JsonWriter writer, SettingItem value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteStartObject();
        writer.WriteString("Name", value.Name);
        writer.WriteString("Type", value.Value.GetType().AssemblyQualifiedName ?? value.Type);
        writer.WritePropertyName("Value");
        JsonSerializer.Serialize(writer, value.Value, value.Value.GetType(), options);
        writer.WriteEndObject();
    }
}
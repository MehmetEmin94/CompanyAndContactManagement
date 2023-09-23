using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CompanyAndContactManagement.HttpApi.Models;

public class ExtraFieldsDictionarySerializer : SerializerBase<Dictionary<string, object>>
{
    public override Dictionary<string, object> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;
        var dict = new Dictionary<string, object>();

        reader.ReadStartDocument();
        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var fieldName = reader.ReadName();
            var fieldValue = ReadValue(reader);
            dict[fieldName] = fieldValue;
        }
        reader.ReadEndDocument();

        return dict;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Dictionary<string, object> value)
    {
        var writer = context.Writer;

        writer.WriteStartDocument();
        foreach (var kvp in value)
        {
            writer.WriteName(kvp.Key);
            WriteValue(writer, kvp.Value);
        }
        writer.WriteEndDocument();
    }

    private static void WriteValue(IBsonWriter writer, object value)
    {
        var json = JsonSerializer.Serialize(value);
        writer.WriteString(json);
    }

    private static object ReadValue(IBsonReader reader)
    {
        var json = reader.ReadString();
        return JsonSerializer.Deserialize<object>(json);
    }
}
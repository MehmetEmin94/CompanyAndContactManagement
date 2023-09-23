using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CompanyAndContactManagement.HttpApi.Models.Contact;


public class Contact
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; }=String.Empty;

    public List<int> CompanyIds { get; set; } = new();

    [BsonSerializer(typeof(ExtraFieldsDictionarySerializer))]
    public Dictionary<string,object> ExtraFields { get; set; } = new ();
}
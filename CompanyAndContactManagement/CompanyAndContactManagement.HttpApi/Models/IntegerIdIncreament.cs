using MongoDB.Bson;
using MongoDB.Driver;

namespace CompanyAndContactManagement.HttpApi.Models;

public static class IntegerIdIncreament
{
    public static int GetNextId(string collectionName, IMongoDatabase database)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", collectionName);
        var update = Builders<BsonDocument>.Update.Inc("seq", 1);
        var options = new FindOneAndUpdateOptions<BsonDocument>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        var result = database.GetCollection<BsonDocument>("counters")
            .FindOneAndUpdate(filter, update, options);

        return result["seq"].AsInt32;
    }
}
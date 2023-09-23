using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace CompanyAndContactManagement.HttpApi.Models.Company;


public class Company
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; }=String.Empty;

    public int NumberOfEmployees { get; set; }

    
    [BsonSerializer(typeof(ExtraFieldsDictionarySerializer))]
    public Dictionary<string,object> ExtraFields { get; set; } = new ();
}






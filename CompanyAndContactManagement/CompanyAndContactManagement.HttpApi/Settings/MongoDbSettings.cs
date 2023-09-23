namespace CompanyAndContactManagement.HttpApi.Settings;

public class MongoDbSettings
{
    
    public string DatabaseName { get; set; }=String.Empty;
    
    public string CollectionName { get; set; }=String.Empty;

    public string ConnectionString { get; set; }=String.Empty;
}
using Newtonsoft.Json.Linq;

namespace CompanyAndContactManagement.HttpApi.DTOs;

public class ExtraFields
{
    public string ColumnName { get; set; }
    
    public dynamic Value { get; set; }
}
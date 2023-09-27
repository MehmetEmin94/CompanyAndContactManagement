using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using CompanyAndContactManagement.HttpApi.Services;

namespace CompanyAndContactManagement.HttpApi.DTOs.Company;

public class GetCompaniesInput 
{
    public CompaniesFilters Filters { get; set; } = new ();
    public string GlobalFilter { get; set; }
}

[TypeConverter(typeof(FiltersConverter<CompaniesFilters>))]
public class CompaniesFilters:IFilters
{
    public FilterMetadata Id { get; set; } = new();
    public FilterMetadata Name { get; set; } = new();
    public FilterMetadata NumberOfEmployees { get; set; } = new();

    public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
}
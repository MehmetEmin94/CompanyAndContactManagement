using CompanyAndContactManagement.HttpApi.Models;

namespace CompanyAndContactManagement.HttpApi.DTOs.Company;

public class CompanyDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }=String.Empty;

    public int NumberOfEmployees { get; set; }

    public Dictionary<string,object> ExtraFields { get; set; } = new ();
}
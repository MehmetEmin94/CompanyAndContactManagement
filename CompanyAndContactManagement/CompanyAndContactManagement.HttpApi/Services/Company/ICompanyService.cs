using CompanyAndContactManagement.HttpApi.DTOs;
using CompanyAndContactManagement.HttpApi.DTOs.Company;
using Newtonsoft.Json.Linq;

namespace CompanyAndContactManagement.HttpApi.Services.Company;




public interface ICompanyService
{
    Task Create(CompanyDto company);
    
    Task<CompanyDto> Update(int id,CompanyDto company);
    
    Task Delete(int id);
    
    Task<CompanyDto> Get(int id);
    
    Task<List<CompanyDto>> GetList(GetCompaniesInput input);
    
    Task<CompanyDto> AddNewFields(int id,List<ExtraFields> fields);
}
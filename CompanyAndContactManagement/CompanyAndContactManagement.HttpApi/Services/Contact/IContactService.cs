using CompanyAndContactManagement.HttpApi.DTOs;
using CompanyAndContactManagement.HttpApi.DTOs.Contact;

namespace CompanyAndContactManagement.HttpApi.Services.Contact;
public interface IContactService
{
    Task Create(ContactDto contact);
    
    Task<ContactDto> Update(int id,ContactDto contact);
    
    Task Delete(int id);
    
    Task<ContactDto> Get(int id);
    
    Task<List<ContactDto>> GetList();
    
    Task<ContactDto> AddNewFields(int id,List<ExtraFields> fields);
}
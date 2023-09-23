using CompanyAndContactManagement.HttpApi.DTOs;
using CompanyAndContactManagement.HttpApi.DTOs.Contact;
using CompanyAndContactManagement.HttpApi.ResponseModels;
using CompanyAndContactManagement.HttpApi.Services.Contact;
using Microsoft.AspNetCore.Mvc;

namespace CompanyAndContactManagement.HttpApi.Controllers.Contact;

[ApiController]
[Route("api/[controller]")]
public class ContactsController:ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost]
    public async Task Create(ContactDto contact) =>
        await _contactService.Create(contact);

    [HttpPut]
    [Route("{id}")]
    public async Task<ServiceResponse<ContactDto>> Update(int id, ContactDto contact)
    {
        return new ServiceResponse<ContactDto>()
        {
            Value = await _contactService.Update(id, contact)
        };
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task Delete(int id)=>
        await _contactService.Delete(id);

    [HttpGet]
    [Route("{id}")]
    public async Task<ServiceResponse<ContactDto>> Get(int id)
    {
        return new ServiceResponse<ContactDto>()
        {
            Value = await _contactService.Get(id)
        };
    }

    [HttpGet]
    public async Task<ServiceResponse<List<ContactDto>>> GetList()
    {
        return new ServiceResponse<List<ContactDto>>()
        {
            Value = await _contactService.GetList()
        };
    }

    [HttpPut]
    [Route("add-new-column/{id}")]
    public async Task<ServiceResponse<ContactDto>> AddNewFields(int id ,[FromBody] List<ExtraFields> fields)
    {
        return new ServiceResponse<ContactDto>()
        {
            Value = await _contactService.AddNewFields(id,fields)
        };
    }
}
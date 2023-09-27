using System.Dynamic;
using CompanyAndContactManagement.HttpApi.DTOs;
using CompanyAndContactManagement.HttpApi.DTOs.Company;
using CompanyAndContactManagement.HttpApi.Services.Company;
using Microsoft.AspNetCore.Mvc;
using CompanyAndContactManagement.HttpApi.ResponseModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CompanyAndContactManagement.HttpApi.Controllers.Company;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController:ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpPost]
    public async Task Create(CompanyDto company) =>
        await _companyService.Create(company);

    [HttpPut]
    [Route("{id}")]
    public async Task<ServiceResponse<CompanyDto>> Update(int id, CompanyDto company)
    {
        return new ServiceResponse<CompanyDto>()
        {
            Value = await _companyService.Update(id, company)
        };
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task Delete(int id) =>
        await _companyService.Delete(id);

    [HttpGet]
    [Route("{id}")]
    public async Task<ServiceResponse<CompanyDto>> Get(int id)
    {
        return new ServiceResponse<CompanyDto>()
        {
            Value = await _companyService.Get(id)
        };
    }

    [HttpGet]
    public async Task<ServiceResponse<List<CompanyDto>>> GetList(GetCompaniesInput input)
    {
        return new ServiceResponse<List<CompanyDto>>()
        {
            Value = await _companyService.GetList(input)
        };
    }

    [HttpPut]
    [Route("add-new-column/{id}")]
    public async Task<ServiceResponse<CompanyDto>> AddNewFields(int id,[FromBody] List<ExtraFields> fields)
    {
        return new ServiceResponse<CompanyDto>()
        {
            Value = await _companyService.AddNewFields(id,fields)
        };
    }
}
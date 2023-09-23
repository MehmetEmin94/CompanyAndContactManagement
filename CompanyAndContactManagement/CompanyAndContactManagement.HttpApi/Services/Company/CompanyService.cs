using AutoMapper;
using CompanyAndContactManagement.HttpApi.DTOs;
using CompanyAndContactManagement.HttpApi.DTOs.Company;
using CompanyAndContactManagement.HttpApi.Models;
using CompanyAndContactManagement.HttpApi.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CompanyAndContactManagement.HttpApi.Services.Company;

using CompanyAndContactManagement.HttpApi.Models.Company;

public class CompanyService:ICompanyService
{
    private readonly IMongoCollection<Company> _mongoCollection;
    private readonly IMapper _mapper;
    public CompanyService(IOptions<MongoDbSettings> mongoDbSettings, IMapper mapper)
    {
        _mapper = mapper;
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _mongoCollection = mongoDb.GetCollection<Company>($"{typeof(Company).Name}");
    }
    public async Task Create(CompanyDto company)
    {
        var creatableCompany = _mapper.Map<CompanyDto, Company>(company);
        await _mongoCollection.InsertOneAsync(creatableCompany);
    }

    public async Task<CompanyDto> Update(int id, CompanyDto company)
    {
        var updatingCompany =(await _mongoCollection.FindAsync(_ => _.Id == id)).FirstOrDefault();
        if (updatingCompany==null)
        {
            throw new Exception("No data found to update");
        }
        updatingCompany= _mapper.Map(company,updatingCompany);
        await _mongoCollection.ReplaceOneAsync(_=>_.Id==id,updatingCompany);
        
        return company;
    }

    public async Task Delete(int id)=>
        await _mongoCollection.DeleteOneAsync(_=>_.Id==id);

    public async Task<CompanyDto> Get(int id) =>
        _mapper.Map<Company, CompanyDto>((await _mongoCollection.FindAsync(_ => true)).FirstOrDefault());

    public async Task<List<CompanyDto>> GetList()
    {
        var companies = await _mongoCollection.Find(_ => true).ToListAsync();
        return _mapper.Map<List<Company>, List<CompanyDto>>(companies);
    }

    public async Task<CompanyDto> AddNewFields(int id,List<ExtraFields> fields)
    {
        var company = (await _mongoCollection.FindAsync(_ => _.Id == id)).FirstOrDefault();

        foreach (var field in fields)
        {
            company.ExtraFields.Add(field.ColumnName,field.Value);
        }
        
        
        await _mongoCollection.ReplaceOneAsync(_=>_.Id==id,company);
        return  _mapper.Map<Company, CompanyDto>(company);
    }
}
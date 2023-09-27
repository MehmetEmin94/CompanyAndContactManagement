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

public class CompanyService:BaseService<Company>,ICompanyService
{
    private readonly IMongoCollection<Company> _mongoCollection;
    private readonly IMapper _mapper;
    private readonly IMongoDatabase _mongo;
    public CompanyService(IOptions<MongoDbSettings> mongoDbSettings, IMapper mapper):base(mongoDbSettings)
    {
        _mapper = mapper;
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        _mongo = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _mongoCollection = _mongo.GetCollection<Company>($"{typeof(Company).Name}");
    }
    public async Task Create(CompanyDto company)
    {
        var creatableCompany = _mapper.Map<CompanyDto, Company>(company);
        creatableCompany.Id = IntegerIdIncreament.GetNextId($"{typeof(Company).Name}", _mongo);
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


    public async Task<List<CompanyDto>> GetList( GetCompaniesInput input)
    {
        var query=await base.GetAggregateAsync();
        
        query = ApplyFilter(query, input.Filters,input.GlobalFilter);
        var companies = await query.ToListAsync();
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
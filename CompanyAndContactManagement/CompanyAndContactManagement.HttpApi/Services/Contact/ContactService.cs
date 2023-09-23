using AutoMapper;
using CompanyAndContactManagement.HttpApi.DTOs;
using CompanyAndContactManagement.HttpApi.DTOs.Contact;
using CompanyAndContactManagement.HttpApi.Models;
using CompanyAndContactManagement.HttpApi.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CompanyAndContactManagement.HttpApi.Services.Contact;
using CompanyAndContactManagement.HttpApi.Models.Contact;
public class ContactService:IContactService
{
    private readonly IMongoCollection<Contact> _mongoCollection;
    private readonly IMapper _mapper;
    private readonly IMongoDatabase _mongo;
    public ContactService(IOptions<MongoDbSettings> mongoDbSettings, IMapper mapper)
    {
        _mapper = mapper;
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        _mongo = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _mongoCollection = _mongo.GetCollection<Contact>($"{typeof(Contact).Name}");
    }
    public async Task Create(ContactDto contact)
    {
        var creatableContact = _mapper.Map<ContactDto, Contact>(contact);
        creatableContact.Id = IntegerIdIncreament.GetNextId($"{typeof(Contact).Name}", _mongo);
        await _mongoCollection.InsertOneAsync(creatableContact);
    }

    public async Task<ContactDto> Update(int id, ContactDto contact)
    {
        var updatingCompany = _mongoCollection.FindSync(_ => _.Id == id).FirstOrDefault();
        if (updatingCompany==null)
        {
            throw new Exception("No data found to update");
        }
        updatingCompany= _mapper.Map(contact,updatingCompany);
        await _mongoCollection.ReplaceOneAsync(_=>_.Id==id,updatingCompany);
        
        return contact;
    }

    public async Task Delete(int id)=>
        await _mongoCollection.DeleteOneAsync(_=>_.Id==id);

    public async Task<ContactDto> Get(int id)=>
        _mapper.Map<Contact, ContactDto>((await _mongoCollection.FindAsync(_ => true)).FirstOrDefault());

    public async Task<List<ContactDto>> GetList()
    {
        var contacts = await _mongoCollection.Find(_ => true).ToListAsync();
        return _mapper.Map<List<Contact>, List<ContactDto>>(contacts);
    }

    public async Task<ContactDto> AddNewFields(int id,List<ExtraFields> fields)
    {
        var contact = (await _mongoCollection.FindAsync(_ => _.Id == id)).FirstOrDefault();

        foreach (var field in fields)
        {
            contact.ExtraFields.Add(field.ColumnName,field.Value);
        }
        
        
        await _mongoCollection.ReplaceOneAsync(_=>_.Id==id,contact);
        return  _mapper.Map<Contact, ContactDto>(contact);
    }
}
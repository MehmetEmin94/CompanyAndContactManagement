using AutoMapper;
using CompanyAndContactManagement.HttpApi.DTOs.Company;
using CompanyAndContactManagement.HttpApi.DTOs.Contact;


namespace CompanyAndContactManagement.HttpApi.Services.Extensions;

using CompanyAndContactManagement.HttpApi.Models.Company;
using CompanyAndContactManagement.HttpApi.Models.Contact;

public static class ConfigureMappingExtension
{
    public static IServiceCollection ConfigureMapping(this IServiceCollection service)
    {
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });

        IMapper mapper = mappingConfig.CreateMapper();

        service.AddSingleton(mapper);

        return service;
    }
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        AllowNullDestinationValues = true;
        AllowNullCollections = true;

        CreateMap<Company, CompanyDto>();

        CreateMap<CompanyDto, Company>();

        CreateMap<Contact, ContactDto>();

        CreateMap<ContactDto, Contact>();


    }
}
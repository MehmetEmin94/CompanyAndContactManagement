namespace CompanyAndContactManagement.HttpApi.DTOs.Contact;

public class ContactDto
{
    public int Id { get; set; }

    public string Name { get; set; }=String.Empty;

    public List<int> CompanyIds { get; set; } = new();

    public Dictionary<string, object> ExtraFields { get; set; } = new ();
}
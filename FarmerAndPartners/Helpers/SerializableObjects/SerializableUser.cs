using System.Xml.Serialization;

namespace FarmerAndPartners.Helpers.SerializableObjects
{
    [XmlType("User")]
    public class SerializableUser
    {
        public SerializableUser() { }
        public SerializableUser(int id, string name, string login, string password, int companyId, string companyName)
        {
            Id = id;
            Name = name;
            Login = login;
            Password = password;
            CompanyId = companyId;
            CompanyName = companyName;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
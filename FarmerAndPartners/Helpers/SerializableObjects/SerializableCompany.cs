namespace FarmerAndPartners.Helpers.SerializableObjects
{
    public class SerializableCompany
    {
        public SerializableCompany() { }
        public SerializableCompany(int id, string name, int contractStatusId, string contractStatusDefinition, UsersCollection users)
        {
            Id = id;
            Name = name;
            ContractStatusId = contractStatusId;
            ContractStatusDefinition = contractStatusDefinition;
            Users = users;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int ContractStatusId { get; set; }
        public string ContractStatusDefinition { get; set; }
        public UsersCollection Users { get; set; }
    }
}

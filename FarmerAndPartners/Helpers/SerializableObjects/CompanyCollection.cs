using System.Collections.Generic;
using System.Xml.Serialization;

namespace FarmerAndPartners.Helpers.SerializableObjects
{
    [XmlRoot("Companies")]
    public class CompanyCollection
    {
        [XmlElement("Company")]
        public List<SerializableCompany> Companies { get; set; }
    }
}

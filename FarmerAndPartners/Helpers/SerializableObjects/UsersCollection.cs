using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FarmerAndPartners.Helpers.SerializableObjects
{
    [JsonArray]
    [XmlRoot("Users")]
    public class UsersCollection : ICollection<SerializableUser>
    {
        public UsersCollection()
        {
            Users = new List<SerializableUser>();
        }

        public List<SerializableUser> Users { get; set; }
        public int Count => Users.Count;
        public bool IsReadOnly => false;
        public void Add(SerializableUser item) => Users.Add(item);
        public void Clear() => Users.Clear();
        public void CopyTo(SerializableUser[] array, int arrayIndex) => Users.CopyTo(array, arrayIndex);
        public IEnumerator<SerializableUser> GetEnumerator() => Users.GetEnumerator();
        public bool Remove(SerializableUser item) => Users.Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public bool Contains(SerializableUser item)
        {
            if (Users.Contains(item))
                return true;
            else
                return false;
        }
    }
}
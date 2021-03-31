using FarmerAndPartnersModels;
using System.Collections.Generic;

namespace FarmerAndPartners.Helpers.BackgroundWorkerArguments
{
    public class UserArguments
    {
        public UserArguments(IEnumerable<User> users, int usersCount)
        {
            Users = users;
            UsersCount = usersCount;
        }

        public IEnumerable<User> Users { get; }
        public int UsersCount { get; }
    }
}

using PostMatch.Core.Entities;
using System.Collections.Generic;

namespace PostMatch.Core.Interface
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        IEnumerable<User> GetAll();
        User GetById(string id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Patch(User user, string password);
        void Delete(string id);
    }
}

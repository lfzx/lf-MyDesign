using PostMatch.Core.Entities;
using System.Collections.Generic;
using System.Data;

namespace PostMatch.Core.Interface
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        DataSet GetAll(string school);
        User GetById(string id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Patch(User user, string password);
        void EditPassword(User user, string password);
        void Delete(string id);
        DataSet GetByIdForMatch(string id);


    }
}

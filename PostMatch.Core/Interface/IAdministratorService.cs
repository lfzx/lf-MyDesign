using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Core.Interface
{
    public interface IAdministratorService
    {
        Administrator Authenticate(string email, string password);
        IEnumerable<Administrator> GetAll();
        Administrator GetById(string id);
        Administrator Create(Administrator administrator, string password);
        void Update(Administrator administrator, string password = null);
        void EditPassword(Administrator administrator, string password);
        void Delete(string id);
    }
}

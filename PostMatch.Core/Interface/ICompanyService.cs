using PostMatch.Core.Entities;
using System.Collections.Generic;
using System.Data;

namespace PostMatch.Core.Interface
{
    public interface ICompanyService
    {
        Companies Authenticate(string email, string password);
        IEnumerable<Companies> GetAll();
        Companies GetById(string id);
        Companies Create(Companies companies, string password);
        void Update(Companies companies, string password = null);
        void Patch(Companies companies, string password);
        void Delete(string id);
        DataSet GetByName(string id);
    }
}

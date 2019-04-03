using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PostMatch.Infrastructure.DataAccess.Interface
{
    public interface IAdministratorRepository
    {
        Administrator GetAdministratorByEmail(string email);
        IEnumerable<Administrator> GetAll();
        Administrator GetById(string id);
        bool Any(Expression<Func<Administrator, bool>> predicate);
        int Add(Administrator administrator);
        int Update(Administrator administrator);
        int Remove(Administrator administrator);
    }
}

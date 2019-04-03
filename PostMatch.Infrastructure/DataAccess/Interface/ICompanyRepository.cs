using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PostMatch.Infrastructure.DataAccess.Interface
{
    public interface ICompanyRepository
    {
        Companies GetCompaniesByEmail(string email);
        IEnumerable<Companies> GetAll();
        Companies GetById(string id);
        bool Any(Expression<Func<Companies, bool>> predicate);
        int Add(Companies companies);
        int Update(Companies companies);
        int Remove(Companies companies);
    }
}

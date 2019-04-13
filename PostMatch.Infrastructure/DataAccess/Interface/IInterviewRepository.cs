using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PostMatch.Infrastructure.DataAccess.Interface
{
    public interface IInterviewRepository
    {
        IEnumerable<Interview> GetAll();
        Interview GetById(string id);
        bool Any(Expression<Func<Interview, bool>> predicate);
        int Add(Interview interview);
        int Update(Interview interview);
        int Remove(Interview interview);
        int Patch(Interview interview);
    }
}

using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PostMatch.Infrastructure.DataAccess.Interface
{
    public interface IResumeRepository
    {
        IEnumerable<Resume> GetAll();
        Resume GetById(string id);
        Resume GetByUserId(string id);
        bool Any(Expression<Func<Resume, bool>> predicate);
        int Add(Resume resume);
        int Update(Resume resume);
        int Remove(Resume resume);
    }
}

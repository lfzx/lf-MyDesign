using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PostMatch.Infrastructure.DataAccess.Interface
{
    public interface IUserRepository
    {
        User GetUserByEmail(string email);
        IEnumerable<User> GetAll();
        User GetById(string id);
        bool Any(Expression<Func<User, bool>> predicate);
        int Add(User user);
        int Update(User user);
        int Remove(User user);
    }
}

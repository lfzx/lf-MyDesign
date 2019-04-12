using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PostMatch.Infrastructure.DataAccess.Interface
{
    public interface IPostRepository
    {
        IEnumerable<Post> GetAll();
        Post GetById(string id);
        //Post GetByCompanyId(string id);
        bool Any(Expression<Func<Post, bool>> predicate);
        int Add(Post post);
        int Update(Post post);
        int Remove(Post post);
    }
}

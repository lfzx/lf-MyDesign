using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PostMatch.Infrastructure.DataAccess.Interface
{
    public interface IRecommendRepository
    {
        IEnumerable<Recommend> GetAll();
        IEnumerable<Recommend> GetIdByPostIdAndResumeId(string postId, string resumeId);
        Recommend GetById(string id);
        bool Any(Expression<Func<Recommend, bool>> predicate);
        int Add(Recommend recommend);
        int Update(Recommend recommend);
        int Patch(Recommend recommend);
        int Remove(Recommend recommend);
    }
}

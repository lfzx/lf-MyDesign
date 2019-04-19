using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Core.Interface
{
    public interface IRecommendService
    {
        IEnumerable<Recommend> GetAll();
        Recommend GetById(string id);
        Recommend Create(Recommend recommend, string postId, string resumeId);
        Recommend CreateForMatch(Recommend recommend, string postId, string companyId);
        void Update(Recommend recommend);
        void Patch(Recommend recommend);
        void Delete(string id);
    }
}

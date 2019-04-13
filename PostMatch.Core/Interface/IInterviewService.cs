using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Core.Interface
{
  public interface IInterviewService
    {
        IEnumerable<Interview> GetAll();
        Interview GetById(string id);
        Interview Create(Interview interview, string postId, string resumeId);
        void Update(Interview interview);
        void Patch(Interview interview);
        void Delete(string id);
    }
}

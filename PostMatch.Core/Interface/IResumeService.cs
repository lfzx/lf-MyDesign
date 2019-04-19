using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PostMatch.Core.Interface
{
    public interface IResumeService
    {
        IEnumerable<Resume> GetAll();
        Resume GetById(string id);
        Resume GetByUserId(string id);
        Resume Create(Resume resume,string userId);
        void Update(Resume resume);
        void Patch(Resume resume, string userId);
        void Delete(string resumeId);
        DataSet GetByResumeForRecommend(string id);
        DataSet GetByIdForDelivery(string id);
        DataSet GetByIdForInterview(string id);
        DataSet GetByIdForUser(string id);

        DataSet GetByName(string name);
    }
}

using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Core.Interface
{
    public interface IResumeService
    {
        IEnumerable<Resume> GetAll();
        Resume GetById(string id);
        Resume Create(Resume resume,string userId);
        void Update(Resume resume);
        void Patch(Resume resume, string userId);
        void Delete(string resumeId);
    }
}

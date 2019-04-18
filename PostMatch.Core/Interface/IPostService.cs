using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PostMatch.Core.Interface
{
    public interface IPostService
    {
        IEnumerable<Post> GetAll();
        Post GetById(string id);
        //Post GetByCompanyId(string id);
        Post Create(Post post, string companyId);
        void Update(Post post);
        void Patch(Post post, string companyId);
        void Delete(string id);

        DataSet GetByName(string name);
    }
}

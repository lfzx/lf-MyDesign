using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using PostMatch.Infrastructure.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Infrastructure.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _iPostRepository;
        private readonly ICompanyRepository _iCompanyRepository;

        public PostService(IPostRepository iPostRepository, ICompanyRepository iCompanyRepository)
        {
            _iPostRepository = iPostRepository;
            _iCompanyRepository = iCompanyRepository;
        }

        public Post Create(Post post, string companyId)
        {
            // 验证
            if (string.IsNullOrWhiteSpace(companyId))
                throw new AppException("公司id不能为空！");

            var company = _iCompanyRepository.GetById(companyId);

            if (company == null)
            {
                throw new AppException("该公司未注册！");
            }

            post.PostId = Guid.NewGuid().ToString();

            post.CompanyId = companyId;
            post.PostName = post.PostName;
            post.PostWorkPlace = post.PostWorkPlace;
            post.PostSalary = post.PostSalary;
            post.PostJobType = post.PostJobType;
            post.PostDescription = post.PostDescription;
            post.City = post.City;
            post.PostExperience = post.PostExperience;
            post.AcademicRequirements = post.AcademicRequirements;
            post.NumberOfRecruits = post.NumberOfRecruits;
            post.PostTelephoneNumber = post.PostTelephoneNumber;
            post.PostEmail = post.PostEmail;
            post.PostUpdateTime = DateTime.Now;

            _iPostRepository.Add(post);

            return post;
        }

        public void Delete(string id)
        {
            var post = _iPostRepository.GetById(id);
            if (post != null)
            {
                _iPostRepository.Remove(post);
            }
        }

        public IEnumerable<Post> GetAll()
        {
            return _iPostRepository.GetAll();
        }

        public Post GetById(string id)
        {
            return _iPostRepository.GetById(id);
        }

        public void Patch(Post post, string companyId)
        {
            throw new NotImplementedException();
        }

        public void Update(Post post)
        {
            var posts = _iPostRepository.GetById(post.PostId);

            if (posts == null)
                throw new AppException("该职位不存在！");

            // update user properties
            posts.PostName = post.PostName;
            posts.PostWorkPlace = post.PostWorkPlace;
            posts.PostSalary = post.PostSalary;
            posts.PostJobType = post.PostJobType;
            posts.PostDescription = post.PostDescription;
            posts.City = post.City;
            posts.PostExperience = post.PostExperience;
            posts.AcademicRequirements = post.AcademicRequirements;
            posts.NumberOfRecruits = post.NumberOfRecruits;
            posts.PostTelephoneNumber = post.PostTelephoneNumber;
            posts.PostEmail = post.PostEmail;
            posts.PostUpdateTime = DateTime.Now;

            _iPostRepository.Update(posts);
        }
    }
}

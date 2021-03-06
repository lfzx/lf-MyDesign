﻿using MySql.Data.MySqlClient;
using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using PostMatch.Infrastructure.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PostMatch.Infrastructure.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _iPostRepository;
        private readonly ICompanyRepository _iCompanyRepository;

        public PostService(
            IPostRepository iPostRepository,
            ICompanyRepository iCompanyRepository)
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

        //public Post GetByCompanyId(string id)
        //{
        //    return _iPostRepository.GetByCompanyId(id);
        //}

        public Post GetById(string id)
        {
            return _iPostRepository.GetById(id);
        }

        public DataSet GetByName(string name)
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT postId,companyId FROM post WHERE postName LIKE ?name";
            MySqlParameter param = new MySqlParameter("?name", MySqlDbType.String);
            param.Value = name;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }

        public void Patch(Post post, string companyId)
        {
            var posts = _iPostRepository.GetById(post.PostId);

            if (posts == null)
                throw new AppException("该职位不存在！");

            var company = _iCompanyRepository.GetById(companyId);
            if (company == null)
            {
                throw new AppException("该公司不存在！");
            }

            //posts.RecommendResumeId = post.RecommendResumeId;
            posts.PostUpdateTime = DateTime.Now;
            _iPostRepository.Update(posts);
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
            posts.PostUpdateTime = DateTime.Now;

            _iPostRepository.Update(posts);
        }
    }
}

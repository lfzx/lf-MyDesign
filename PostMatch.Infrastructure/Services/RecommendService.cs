﻿using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using PostMatch.Infrastructure.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Infrastructure.Services
{
    public class RecommendService : IRecommendService
    {
        private readonly IRecommendRepository _iRecommendRepository;
        private readonly IPostRepository _iPostRepository;
        private readonly IResumeRepository _iResumeRepository;

        public RecommendService(
            IRecommendRepository iRecommendRepository,
            IPostRepository iPostRepository,
            IResumeRepository iResumeRepository
            )
        {
            _iRecommendRepository = iRecommendRepository;
            _iPostRepository = iPostRepository;
            _iResumeRepository = iResumeRepository;
        }

        public Recommend Create(Recommend recommend, string postId, string resumeId)
        {
            // 验证
            if (string.IsNullOrWhiteSpace(postId))
                throw new AppException("职位id不能为空！");

            if (string.IsNullOrWhiteSpace(resumeId))
                throw new AppException("简历id不能为空！");

            var post = _iPostRepository.GetById(postId);

            if (post == null)
            {
                throw new AppException("该职位不存在！");
            }

            var resume = _iResumeRepository.GetById(resumeId);

            if (resume == null)
            {
                throw new AppException("该简历不存在！");
            }

            recommend.RecommendId = Guid.NewGuid().ToString();

            recommend.PostId = postId;
            recommend.ResumeId = resumeId;
            recommend.RecommendNumber = recommend.RecommendNumber;
            recommend.RecommendUpdateTime = DateTime.Now;

            _iRecommendRepository.Add(recommend);

            return recommend;
        }

        public void Delete(string id)
        {
            var recommend = _iRecommendRepository.GetById(id);
            if (recommend != null)
            {
                _iRecommendRepository.Remove(recommend);
            }
        }

        public IEnumerable<Recommend> GetAll()
        {
            return _iRecommendRepository.GetAll();
        }

        public Recommend GetById(string id)
        {
            return _iRecommendRepository.GetById(id);
        }

        public void Patch(Recommend recommend, string userId)
        {
            throw new NotImplementedException();
        }

        public void Update(Recommend recommend)
        {
            var recommends = _iRecommendRepository.GetById(recommend.RecommendId);

            if (recommends == null)
                throw new AppException("该推荐不存在！");

            var post = _iPostRepository.GetById(recommend.PostId);
            if (post == null)
            {
                throw new Exception("该职位不存在！");
            }

            var resume = _iResumeRepository.GetById(recommend.RecommendId);
            if (resume == null)
            {
                throw new Exception("该简历不存在！");
            }

            // update user properties
            recommends.PostId = recommend.PostId;
            recommends.ResumeId = recommend.ResumeId;
            recommends.RecommendNumber = recommend.RecommendNumber;
            recommends.RecommendUpdateTime = DateTime.Now;
            
            _iRecommendRepository.Update(recommends);
        }
    }
}
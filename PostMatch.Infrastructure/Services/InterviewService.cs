using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using PostMatch.Infrastructure.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Infrastructure.Services
{
     public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _iInterviewRepository;
        private readonly IPostRepository _iPostRepository;
        private readonly IResumeRepository _iResumeRepository;

        public InterviewService(
            IInterviewRepository iInterviewRepository,
            IPostRepository iPostRepository,
            IResumeRepository iResumeRepository
            )
        {
            _iInterviewRepository = iInterviewRepository;
            _iPostRepository = iPostRepository;
            _iResumeRepository = iResumeRepository;
        }

        public Interview Create(Interview interview, string postId, string resumeId)
        {
            // 验证
            if (string.IsNullOrWhiteSpace(resumeId))
                throw new AppException("简历id不能为空！");
            if (string.IsNullOrWhiteSpace(postId))
                throw new AppException("职位id不能为空！");

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

            if (interview.CompanyId == null)
            {
                throw new AppException("该公司不存在！");
            }

            interview.InterviewId = Guid.NewGuid().ToString();

            interview.PostId = postId;
            interview.ResumeId = resumeId;
            interview.CompanyId = interview.CompanyId;
            interview.InterviewUpdateTime = DateTime.Now;

            _iInterviewRepository.Add(interview);

            return interview;
        }

        public void Delete(string id)
        {
            var interview = _iInterviewRepository.GetById(id);
            if (interview != null)
            {
                _iInterviewRepository.Remove(interview);
            }
        }

        public IEnumerable<Interview> GetAll()
        {
            return _iInterviewRepository.GetAll();
        }

        public Interview GetById(string id)
        {
            return _iInterviewRepository.GetById(id);
        }

        public void Patch(Interview interview)
        {
            var interviews = _iInterviewRepository.GetById(interview.InterviewId);

            if (interviews == null)
                throw new AppException("该面试邀请不存在！");

            // update user properties
            if (interview.PostId != null)
            {
                interviews.PostId = interview.PostId;
            }
            if (interview.ResumeId != null)
            {
                interviews.ResumeId = interview.ResumeId;
            }
            if (interview.CompanyId != null)
            {
                interviews.CompanyId = interview.CompanyId;
            }
            interviews.PostId = interviews.PostId;
            interviews.ResumeId = interviews.ResumeId;
            interviews.CompanyId = interviews.CompanyId;
            interviews.UserResponse = interview.UserResponse;
            interviews.InterviewUpdateTime = DateTime.Now;

            _iInterviewRepository.Patch(interviews);
        }

        public void Update(Interview interview)
        {
            var interviews = _iInterviewRepository.GetById(interview.InterviewId);

            if (interviews == null)
                throw new AppException("该面试邀请不存在！");

            // update user properties
            interviews.PostId = interview.PostId;
            interviews.ResumeId = interview.ResumeId;
            interviews.CompanyId = interview.CompanyId;
            interviews.InterviewUpdateTime = DateTime.Now;

            _iInterviewRepository.Update(interviews);
        }
    }
    }

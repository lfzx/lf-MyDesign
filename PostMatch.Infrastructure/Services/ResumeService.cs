﻿using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using PostMatch.Infrastructure.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Infrastructure.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IResumeRepository _iResumeRepository;
        private readonly IUserRepository _iUserRepository;

        public ResumeService(IResumeRepository iResumeRepository,IUserRepository iUserRepository)
        {
            _iResumeRepository = iResumeRepository;
            _iUserRepository = iUserRepository;
        }

        public Resume Create(Resume resume, string userId)
        {
            // 验证
            if (string.IsNullOrWhiteSpace(userId))
                throw new AppException("用户id不能为空！");

            var user = _iUserRepository.GetById(userId);

            if (user == null)
            {
                throw new AppException("该用户不存在！");
            }

            resume.ResumeId = Guid.NewGuid().ToString();
            if (resume.ResumeAvatar == null)
            {
                resume.ResumeAvatar = "https://ng-alain.com/assets/img/logo-color.svg";
            }
            resume.UserId = userId;
            resume.ResumeUpdateTime = DateTime.Now;
            resume.Gender = resume.Gender;
            resume.ResumeTelephoneNumber = resume.ResumeTelephoneNumber;
            resume.FamilyAddress = resume.FamilyAddress;
            resume.ResumePostName = resume.ResumePostName;
            resume.ResumeSalary = resume.ResumeSalary;
            resume.ResumeWorkPlace = resume.ResumeWorkPlace;
            resume.Academic = resume.Academic;
            resume.ResumeJobType = resume.ResumeJobType;
            resume.ResumeExperience = resume.ResumeExperience;
            resume.Skill = resume.Skill;
            //resume.Email = resume.Email,Name
            resume.IsEnable = 1;

            _iResumeRepository.Add(resume);

            return resume;
        }

        public void Delete(string resumeId)
        {
            var resume = _iResumeRepository.GetById(resumeId);
            if (resume != null)
            {
                _iResumeRepository.Remove(resume);
            }
        }

        public IEnumerable<Resume> GetAll()
        {
            return _iResumeRepository.GetAll();
        }

        public Resume GetById(string id)
        {
            return _iResumeRepository.GetById(id);
        }

        public void Patch(Resume resume, string userId)
        {
            var resumes = _iResumeRepository.GetById(resume.ResumeId);

            if (resumes == null)
                throw new AppException("该简历不存在！");

            var user = _iUserRepository.GetById(userId);
            if(user == null)
            {
                throw new AppException("该用户不存在！");
            }

            // update user properties
            resume.ResumeId = Guid.NewGuid().ToString();
            if (resume.ResumeAvatar == null)
            {
                resumes.ResumeAvatar = "https://ng-alain.com/assets/img/logo-color.svg";
            }
            resumes.UserId = userId;
            resumes.ResumeUpdateTime = DateTime.Now;
            resumes.Gender = resume.Gender;
            resumes.ResumeTelephoneNumber = resume.ResumeTelephoneNumber;
            resumes.FamilyAddress = resume.FamilyAddress;
            resumes.ResumePostName = resume.ResumePostName;
            resumes.ResumeSalary = resume.ResumeSalary;
            resumes.ResumeWorkPlace = resume.ResumeWorkPlace;
            resumes.Academic = resume.Academic;
            resumes.ResumeJobType = resume.ResumeJobType;
            resumes.ResumeExperience = resume.ResumeExperience;
            resumes.Skill = resume.Skill;
            resumes.IsEnable = 1;

            _iResumeRepository.Update(resume);
        }

        public void Update(Resume resume)
        {
            var resumes = _iResumeRepository.GetById(resume.ResumeId);

            if (resumes == null)
                throw new AppException("该简历不存在！");

            // update user properties
            if (resume.ResumeAvatar == null)
            {
                resumes.ResumeAvatar = "https://ng-alain.com/assets/img/logo-color.svg";
            }
            resumes.ResumeUpdateTime = DateTime.Now;
            resumes.Gender = resume.Gender;
            resumes.ResumeTelephoneNumber = resume.ResumeTelephoneNumber;
            resumes.FamilyAddress = resume.FamilyAddress;
            resumes.ResumePostName = resume.ResumePostName;
            resumes.ResumeSalary = resume.ResumeSalary;
            resumes.ResumeWorkPlace = resume.ResumeWorkPlace;
            resumes.Academic = resume.Academic;
            resumes.ResumeJobType = resume.ResumeJobType;
            resumes.ResumeExperience = resume.ResumeExperience;
            resumes.Skill = resume.Skill;
            resumes.IsEnable = 1;

            _iResumeRepository.Update(resumes);
        }
    }
}
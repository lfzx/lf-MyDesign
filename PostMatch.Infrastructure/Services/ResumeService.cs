using MySql.Data.MySqlClient;
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
            resume.Birth = resume.Birth;
            resume.WorkYear= resume.WorkYear;
            resume.ResumeUpdateTime = DateTime.Now;
            resume.ResumeTelephoneNumber = resume.ResumeTelephoneNumber;
            resume.FamilyAddress = resume.FamilyAddress;
            resume.ResumePostName = resume.ResumePostName;
            resume.ResumeSalary = resume.ResumeSalary;
            resume.ResumeWorkPlace = resume.ResumeWorkPlace;
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

        public DataSet GetByResumeForRecommend(string id)
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT recommendId,postName,companyName,resumePostName,recommendNumber,score,recommendUpdateTime" +
                " FROM recommend left JOIN post on post.postId = recommend.postId LEFT JOIN resume on" +
                " recommend.resumeId = resume.resumeId left JOIN companies on recommend.companyId =" +
                " companies.companyId WHERE recommend.resumeId = resume.resumeId and recommend.resumeId=?id";
            MySqlParameter param = new MySqlParameter("?id", MySqlDbType.String);
            param.Value = id;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }

        public Resume GetByUserId(string id)
        {
            return _iResumeRepository.GetByUserId(id);
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

            //resumes.RecommendPostId = resume.RecommendPostId;
            resumes.ResumeUpdateTime = DateTime.Now;
            _iResumeRepository.Update(resumes);
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
            resumes.ResumeTelephoneNumber = resume.ResumeTelephoneNumber;
            resumes.FamilyAddress = resume.FamilyAddress;
            resumes.ResumePostName = resume.ResumePostName;
            resumes.ResumeSalary = resume.ResumeSalary;
            resumes.ResumeWorkPlace = resume.ResumeWorkPlace;
            resumes.Birth = resume.Birth;
            resumes.ResumeJobType = resume.ResumeJobType;
            resumes.ResumeExperience = resume.ResumeExperience;
            resumes.Skill = resume.Skill;
            resumes.WorkYear = resume.WorkYear;
            resumes.IsEnable = 1;

            _iResumeRepository.Update(resumes);
        }

        public DataSet GetByIdForDelivery(string id)
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT deliveryId,postName,companyName,resumePostName,companyResponse,deliveryUpdateTime" +
                     " FROM deliveries left JOIN post on post.postId = deliveries.postId LEFT JOIN resume on" +
                     " deliveries.resumeId = resume.resumeId left JOIN companies on deliveries.companyId =" +
                     " companies.companyId WHERE deliveries.resumeId = resume.resumeId and deliveries.resumeId=?id";
            MySqlParameter param = new MySqlParameter("?id", MySqlDbType.String);
            param.Value = id;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }

        public DataSet GetByIdForInterview(string id)
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT interviewId,postName,companyName,name,userResponse,interviewUpdateTime" +
                     " FROM interviews left JOIN post on post.postId = interviews.postId LEFT JOIN resume on" +
                     " interviews.resumeId = resume.resumeId left JOIN companies on interviews.companyId =" +
                     " companies.companyId LEFT JOIN user on user.id = resume.userId WHERE interviews.resumeId = resume.resumeId and interviews.resumeId=?id";
            MySqlParameter param = new MySqlParameter("?id", MySqlDbType.String);
            param.Value = id;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }

        public DataSet GetByIdForUser(string id)
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT * from resume r INNER JOIN user u ON r.userId = u.id WHERE resumeId =?id";
            MySqlParameter param = new MySqlParameter("?id", MySqlDbType.String);
            param.Value = id;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }

        public DataSet GetByName(string name)
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT resumeId FROM resume WHERE resumePostName LIKE ?name";
            MySqlParameter param = new MySqlParameter("?name", MySqlDbType.String);
            param.Value = name;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }
    }
}

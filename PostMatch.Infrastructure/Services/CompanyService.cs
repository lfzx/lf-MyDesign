using MySql.Data.MySqlClient;
using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using PostMatch.Infrastructure.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Data;

namespace PostMatch.Infrastructure.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _iCompanyRepository;

        public CompanyService(ICompanyRepository iCompanyRepository)
        {
            _iCompanyRepository = iCompanyRepository;
        }

        public Companies Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _iCompanyRepository.GetCompaniesByEmail(email);

            // 检查用户名是否存在
            if (user == null)
                return null;

            //检查公司注册状态
            if(user.Status == 0)
            {
                return user;
            }

            // 检查密码是否正确
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // 认证成功
            return user;
        }

        public IEnumerable<Companies> GetAll()
        {
            return _iCompanyRepository.GetAll();
        }

        public Companies GetById(string id)
        {
            return _iCompanyRepository.GetById(id);
        }

        public Companies Create(Companies companies, string password)
        {
            // 验证
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("密码不能为空！");

            if (string.IsNullOrEmpty(companies.OrganizationCode.Trim()))
                throw new AppException("组织机构代码不能为空！");

            if (string.IsNullOrEmpty(companies.CompanyName.Trim()))
                throw new AppException("用户名不能为空！");

            if (string.IsNullOrEmpty(companies.Email.Trim()))
                throw new AppException("邮箱不能为空！");

            //if (_iUserRepository.Any(x => x.Email == user.Email))
            //    throw new AppException("Email \"" + user.Email + "\" is already taken");

            //if (_iUserRepository.Any(x => x.UserName == user.UserName))
            //    throw new AppException("Username \"" + user.UserName + "\" is already taken");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            companies.CompanyId = Guid.NewGuid().ToString();
            if (companies.RoleId == 0)
            {
                companies.RoleId = 3;
            }
            companies.Status = companies.Status;
            companies.UpdateTime = DateTime.Now;
            if (companies.Avatar == null)
            {
                companies.Avatar = "https://ng-alain.com/assets/img/logo-color.svg";
            }
            companies.PasswordHash = passwordHash;
            companies.PasswordSalt = passwordSalt;

            _iCompanyRepository.Add(companies);

            return companies;
        }

        public void Update(Companies companies, string password = null)
        {
            var user = _iCompanyRepository.GetById(companies.CompanyId);

            if (user == null)
                throw new AppException("该公司未注册！");

            //if (userParam.Name != user.Name)
            //{
            //    //// username has changed so check if the new username is already taken
            //    //if (_iUserRepository.Any(x => x.Name == userParam.Name))
            //    //    throw new AppException("Username " + userParam.Name + " is already taken");
            //}

            if (companies.CompanyName == user.CompanyName)
            {
                throw new AppException("公司名已存在！");
            }
            if (companies.Avatar == null)
            {
                user.Avatar = user.Avatar;
            }
            if (companies.CompanyDescription == null)
            {
                user.CompanyUrl = user.CompanyUrl;
            }
            if (companies.CompanyName == null)
            {
                user.CompanyName = user.CompanyName;
            }
            if (companies.Email == null)
            {
                user.Email = user.Email;
            }
            // update user properties
            user.CompanyDescription = companies.CompanyDescription;
            user.Avatar = companies.Avatar;
            user.CompanyUrl = companies.CompanyUrl;
            user.Email = companies.Email;
            user.CompanyName = companies.CompanyName;
            user.UpdateTime = DateTime.Now;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _iCompanyRepository.Update(user);
        }

        public void Patch(Companies companies, string password)
        {
            var user = _iCompanyRepository.GetById(companies.CompanyId);

            if (user == null)
                throw new AppException("该用户不存在！");

            //if (userParam.Name != user.Name)
            //{
            //    //// username has changed so check if the new username is already taken
            //    //if (_iUserRepository.Any(x => x.Name == userParam.Name))
            //    //    throw new AppException("Username " + userParam.Name + " is already taken");
            //}
            if (password == null)
            {
                user.PasswordHash = user.PasswordHash;
                user.PasswordSalt = user.PasswordSalt;
            }
            else
            {
                CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            // update user properties
            user.CompanyDescription = companies.CompanyDescription;
            if (user.Avatar == null)
            {
                user.Avatar = user.Avatar;
            }
            user.Avatar = companies.Avatar;
            user.Email = companies.Email;
            user.Status = companies.Status;
            user.CompanyUrl = companies.CompanyUrl;
            user.CompanyName = companies.CompanyName;
            user.UpdateTime = DateTime.Now;

            _iCompanyRepository.Update(user);
        }

        public void EditPassword(Companies companies, string password)
        {
            var users = _iCompanyRepository.GetById(companies.CompanyId);

            if (users == null)
                throw new AppException("该公司未注册！");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            companies.PasswordHash = passwordHash;
            companies.PasswordSalt = passwordSalt;

            // update user properties
            users.PasswordHash = companies.PasswordHash;
            users.PasswordSalt = companies.PasswordSalt;
            users.UpdateTime = DateTime.Now;

            _iCompanyRepository.Update(users);
        }

        public void Delete(string id)
        {
            var user = _iCompanyRepository.GetById(id);
            if (user != null)
            {
                _iCompanyRepository.Remove(user);
            }
        }

        // 私人助手方法

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        DataSet ICompanyService.GetByName(string id)
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "select * from post where companyId=?id";
            MySqlParameter param = new MySqlParameter("?id", MySqlDbType.String);
            param.Value = id;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }

        public DataSet GetByIdForDelivery(string id)
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT deliveryId,postName,companyName,name,resumePostName,companyResponse,deliveryUpdateTime" +
                     " FROM deliveries left JOIN post on post.postId = deliveries.postId LEFT JOIN resume on" +
                     " deliveries.resumeId = resume.resumeId left JOIN companies on deliveries.companyId =" +
                     " companies.companyId LEFT JOIN user on user.id = resume.userId WHERE deliveries.companyId = companies.companyId and deliveries.companyId=?id";
            MySqlParameter param = new MySqlParameter("?id", MySqlDbType.String);
            param.Value = id;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }

        public DataSet GetByIdForRecommend(string id)
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT recommendId,postName,companyName,name,resumePostName,recommendNumber,score,recommendUpdateTime" +
                " FROM recommend left JOIN post on post.postId = recommend.postId LEFT JOIN resume on" +
                " recommend.resumeId = resume.resumeId left JOIN companies on recommend.companyId =" +
                " companies.companyId LEFT JOIN user on user.id = resume.userId WHERE recommend.companyId = companies.companyId and recommend.companyId=?id";
            MySqlParameter param = new MySqlParameter("?id", MySqlDbType.String);
            param.Value = id;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }

        public DataSet GetByIdForInterview(string id)
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT interviewId,postName,companyName,name,resumePostName,userResponse,interviewUpdateTime" +
                     " FROM interviews left JOIN post on post.postId = interviews.postId LEFT JOIN resume on" +
                     " interviews.resumeId = resume.resumeId left JOIN companies on interviews.companyId =" +
                     " companies.companyId LEFT JOIN user on user.id = resume.userId WHERE interviews.companyId = companies.companyId and interviews.companyId=?id";
            MySqlParameter param = new MySqlParameter("?id", MySqlDbType.String);
            param.Value = id;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }

    }
}

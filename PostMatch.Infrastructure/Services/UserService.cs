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
    public class UserService : IUserService
    {
        private readonly IUserRepository _iUserRepository;

        public UserService(IUserRepository iUserRepository)
        {
            _iUserRepository = iUserRepository;
        }

        //登录时调用
        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _iUserRepository.GetUserByEmail(email);

            // 检查用户名是否存在
            if (user == null)
                return null;

            //检查用户使用状态
            if (user.IsEnable == 0)
            {
                return null;
            }

            // 检查密码是否正确
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // 认证成功
            return user;
        }

        public DataSet GetAll(string school)
        {
            //return _iUserRepository.GetAll();
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT * FROM user WHERE school=?school";
            MySqlParameter param = new MySqlParameter("?school", MySqlDbType.String);
            param.Value = school;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }

        public User GetById(string id)
        {
            return _iUserRepository.GetById(id);
        }

        public User Create(User user, string password)
        {
            // 验证
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("密码不能为空！");

            if (string.IsNullOrEmpty(user.Name.Trim()))
                throw new AppException("用户名不能为空！");

            if (string.IsNullOrEmpty(user.Email.Trim()))
                throw new AppException("邮箱不能为空！");

            //if (_iUserRepository.Any(x => x.Email == user.Email))
            //    throw new AppException("Email \"" + user.Email + "\" is already taken");

            //if (_iUserRepository.Any(x => x.UserName == user.UserName))
            //    throw new AppException("Username \"" + user.UserName + "\" is already taken");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.Id = Guid.NewGuid().ToString();
            if (user.Avatar == null)
            {
                user.Avatar = "https://ng-alain.com/assets/img/logo-color.svg";
            }
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.UpdateTime = DateTime.Now;
            user.School = user.School;
            user.EntranceTime = user.EntranceTime;
            user.GraduationTime = user.GraduationTime;
            user.Academic = user.Academic;
            user.Profession = user.Profession;
            user.Gender = user.Gender;
            user.RoleId = 2;
            user.IsEnable = 1;
            _iUserRepository.Add(user);

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _iUserRepository.GetById(userParam.Id);

            if (user == null)
                throw new AppException("该用户不存在！");

            //if (userParam.Name != user.Name)
            //{
            //    //// username has changed so check if the new username is already taken
            //    //if (_iUserRepository.Any(x => x.Name == userParam.Name))
            //    //    throw new AppException("Username " + userParam.Name + " is already taken");
            //}

            if(userParam.Name == user.Name)
            {
                throw new AppException("用户名已存在！");
            }
            if (userParam.Avatar == null)
            {
                user.Avatar = user.Avatar;
            }
            if (userParam.Gender == user.Gender)
            {
                user.Gender = user.Gender;
            }
            if (userParam.Email == null)
            {
                user.Email = user.Email;
            }
            // update user properties
            user.RoleId = userParam.RoleId;
            user.Gender = userParam.Gender;
            user.Avatar = userParam.Avatar;
            user.Name = userParam.Name;
            user.UpdateTime = DateTime.Now;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _iUserRepository.Update(user);
        }

        public void Patch(User userParam, string password)
        {
            var user = _iUserRepository.GetById(userParam.Id);

            if (user == null)
                throw new AppException("该用户不存在！");

            //if (userParam.Name != user.Name)
            //{
            //    //// username has changed so check if the new username is already taken
            //    //if (_iUserRepository.Any(x => x.Name == userParam.Name))
            //    //    throw new AppException("Username " + userParam.Name + " is already taken");
            //}
            if (password == null){
                user.PasswordHash = user.PasswordHash;
                user.PasswordSalt = user.PasswordSalt;
            }
            else {
                CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            // update user properties
            user.RoleId = userParam.RoleId;
            user.Name = userParam.Name;
            user.IsEnable = user.IsEnable;
            user.Email = user.Email;
            user.UpdateTime = DateTime.Now;

            _iUserRepository.Update(user);
        }

        public void EditPassword(User user,string password)
        {
            var users = _iUserRepository.GetById(user.Id);

            if (users == null)
                throw new AppException("该用户不存在！");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // update user properties
            users.PasswordHash = user.PasswordHash;
            users.PasswordSalt = user.PasswordSalt;
            users.UpdateTime = DateTime.Now;

            _iUserRepository.Update(users);
        }

        public void Delete(string id)
        {
            var user = _iUserRepository.GetById(id);
            if (user != null)
            {
                _iUserRepository.Remove(user);
            }
        }

        // 私人助手方法
        //创建hash和salt值
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
        //对比hash和salt值
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

        public DataSet GetByIdForMatch(string id)
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT resumeId,userId,familyAddress,resumePostName,resumeSalary,resumeWorkPlace,resumeJobType,resumeExperience,skill,birth, workYear,profession,academic" +
                     " FROM resume left JOIN user on user.id = resume.userId" +
                     " WHERE resume.userId=?id";
            MySqlParameter param = new MySqlParameter("?id", MySqlDbType.String);
            param.Value = id;
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText, param);

            return dataSet;
        }

    }
}

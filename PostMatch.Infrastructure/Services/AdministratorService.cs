using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using PostMatch.Infrastructure.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Infrastructure.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly IAdministratorRepository _iAdministratorRepository;

        public AdministratorService(IAdministratorRepository iAdministratorRepository)
        {
            _iAdministratorRepository = iAdministratorRepository;
        }

        public Administrator Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _iAdministratorRepository.GetAdministratorByEmail(email);

            // 检查用户名是否存在
            if (user == null)
                return null;

            // 检查密码是否正确
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // 认证成功
            return user;
        }

        public IEnumerable<Administrator> GetAll()
        {
            return _iAdministratorRepository.GetAll();
        }

        public Administrator GetById(string id)
        {
            return _iAdministratorRepository.GetById(id);
        }

        public Administrator Create(Administrator user, string password)
        {
            // 验证
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("密码不能为空！");

            if (string.IsNullOrEmpty(user.AdminName.Trim()))
                throw new AppException("用户名不能为空！");

            if (string.IsNullOrEmpty(user.Email.Trim()))
                throw new AppException("邮箱不能为空！");

            //if (_iUserRepository.Any(x => x.Email == user.Email))
            //    throw new AppException("Email \"" + user.Email + "\" is already taken");

            //if (_iUserRepository.Any(x => x.UserName == user.UserName))
            //    throw new AppException("Username \"" + user.UserName + "\" is already taken");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.AdminId = Guid.NewGuid().ToString();
            user.RoleId = 1;
            if (user.Avatar == null)
            {
                user.Avatar = "https://ng-alain.com/assets/img/logo-color.svg";
            }
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.UpdateTime = DateTime.Now;
        
            _iAdministratorRepository.Add(user);

            return user;
        }

        public void Update(Administrator userParam, string password = null)
        {
            var user = _iAdministratorRepository.GetById(userParam.AdminId);

            if (user == null)
                throw new AppException("该用户不存在！");

            //if (userParam.Name != user.Name)
            //{
            //    //// username has changed so check if the new username is already taken
            //    //if (_iUserRepository.Any(x => x.Name == userParam.Name))
            //    //    throw new AppException("Username " + userParam.Name + " is already taken");
            //}

            if (userParam.AdminName == user.AdminName)
            {
                throw new AppException("用户名已存在！");
            }

            // update user properties
            user.School = userParam.School;
            user.Avatar = userParam.Avatar;
            user.UpdateTime = DateTime.Now;
            user.AdminName = userParam.AdminName;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _iAdministratorRepository.Update(user);
        }

        public void Delete(string id)
        {
            var user = _iAdministratorRepository.GetById(id);
            if (user != null)
            {
                _iAdministratorRepository.Remove(user);
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
    }
}

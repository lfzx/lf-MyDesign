using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Services;
using PostMatch.Infrastructure.DataAccess.Interface;
using System;
using System.Collections.Generic;

namespace PostMatch.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _iUserRepository;

        public UserService(IUserRepository iUserRepository)
        {
            _iUserRepository = iUserRepository;
        }

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _iUserRepository.GetUserByEmail(email);

            // 检查用户名是否存在
            if (user == null)
                return null;

            // 检查密码是否正确
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // 认证成功
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _iUserRepository.GetAll();
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

            if (string.IsNullOrEmpty(user.UserName.Trim()))
                throw new AppException("用户名不能为空！");

            if (string.IsNullOrEmpty(user.Email.Trim()))
                throw new AppException("邮箱不能为空！");

            //if (_iUserRepository.Any(x => x.Email == user.Email))
            //    throw new AppException("Email \"" + user.Email + "\" is already taken");

            //if (_iUserRepository.Any(x => x.UserName == user.UserName))
            //    throw new AppException("Username \"" + user.UserName + "\" is already taken");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.Id = Guid.NewGuid().ToString();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _iUserRepository.Add(user);

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _iUserRepository.GetById(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            //if (userParam.UserName != user.UserName)
            //{
            //    // username has changed so check if the new username is already taken
            //    if (_iUserRepository.Any(x => x.UserName == userParam.UserName))
            //        throw new AppException("Username " + userParam.UserName + " is already taken");
            //}

            // update user properties
            user.FirstName = userParam.FirstName;
            user.Avatar = userParam.Avatar;
            user.UserName = userParam.UserName;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _iUserRepository.Update(user);
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

﻿using MySql.Data.MySqlClient;
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
            if(user.RoleId == 0)
            {
                user.RoleId = 2;
            }
            if (user.Avatar == null)
            {
                user.Avatar = "https://ng-alain.com/assets/img/logo-color.svg";
            }
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.UpdateTime = DateTime.Now;
            user.IsEnable = user.IsEnable;
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

            // update user properties
            user.RoleId = userParam.RoleId;
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

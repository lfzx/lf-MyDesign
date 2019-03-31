using Microsoft.EntityFrameworkCore;
using PostMatch.Infrastructure.DataAccess.Interface;
using PostMatch.Infrastructure.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PostMatch.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostMatch.Infrastructure.DataAccess.Implement
{
    public class UserRepository : IUserRepository
    {
        private readonly MyContext _context;

        public UserRepository(MyContext context)
        {
            _context = context;
        }

        public User GetUserByEmail(string email)
        {
            return _context.User.SingleOrDefault(x => x.Email == email);
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User;
        }

        public User GetById(string id)
        {
            return _context.User.Find(id);
        }

        public bool Any(Expression<Func<User, bool>> predicate)
        {
            return _context.User.Any(predicate);
        }

        public int Add(User user)
        {
            _context.User.Add(user);
            return _context.SaveChanges();
        }

        public int Update(User user)
        {
            _context.User.Update(user);
            return _context.SaveChanges();
        }

        public int Remove(User user)
        {
            _context.User.Remove(user);
            return _context.SaveChanges();
        }
    }
}

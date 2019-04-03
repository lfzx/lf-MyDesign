using PostMatch.Core.Entities;
using PostMatch.Infrastructure.DataAccess.Interface;
using PostMatch.Infrastructure.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PostMatch.Infrastructure.DataAccess.Implement
{
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly MyContext _context;

        public AdministratorRepository(MyContext context)
        {
            _context = context;
        }

        public Administrator GetAdministratorByEmail(string email)
        {
            return _context.Administrator.SingleOrDefault(x => x.Email == email);
        }

        public IEnumerable<Administrator> GetAll()
        {
            return _context.Administrator;
        }

        public Administrator GetById(string id)
        {
            return _context.Administrator.Find(id);
        }

        public bool Any(Expression<Func<Administrator, bool>> predicate)
        {
            return _context.Administrator.Any(predicate);
        }

        public int Add(Administrator administrator)
        {
            _context.Administrator.Add(administrator);
            return _context.SaveChanges();
        }

        public int Remove(Administrator administrator)
        {
            _context.Administrator.Remove(administrator);
            return _context.SaveChanges();
        }

        public int Update(Administrator administrator)
        {
            _context.Administrator.Update(administrator);
            return _context.SaveChanges();
        }
    }
}

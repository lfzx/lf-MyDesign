using PostMatch.Core.Entities;
using PostMatch.Infrastructure.DataAccess.Interface;
using PostMatch.Infrastructure.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PostMatch.Infrastructure.DataAccess.Implement
{
    public class CompanyUserRepository : ICompanyRepository
    {
        private readonly MyContext _context;

        public CompanyUserRepository(MyContext context)
        {
            _context = context;
        }

        public Companies GetCompaniesByEmail(string email)
        {
            return _context.Companies.SingleOrDefault(x => x.Email == email);
        }

        public IEnumerable<Companies> GetAll()
        {
            return _context.Companies;
        }

        public Companies GetById(string id)
        {
            return _context.Companies.Find(id);
        }

        public bool Any(Expression<Func<Companies, bool>> predicate)
        {
            return _context.Companies.Any(predicate);
        }

        public int Add(Companies companies)
        {
            _context.Companies.Add(companies);
            return _context.SaveChanges();
        }

        public int Remove(Companies companies)
        {
            _context.Companies.Remove(companies);
            return _context.SaveChanges();
        }

        public int Update(Companies companies)
        {
            _context.Companies.Update(companies);
            return _context.SaveChanges();
        }
    }
}

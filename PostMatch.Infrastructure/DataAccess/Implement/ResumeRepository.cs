using Microsoft.EntityFrameworkCore;
using PostMatch.Core.Entities;
using PostMatch.Infrastructure.DataAccess.Interface;
using PostMatch.Infrastructure.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PostMatch.Infrastructure.DataAccess.Implement
{
    public class ResumeRepository : IResumeRepository
    {
        private readonly MyContext _context;

        public ResumeRepository(MyContext context)
        {
            _context = context;
        }

        public int Add(Resume resume)
        {
            _context.Resume.Add(resume);
            return _context.SaveChanges();
        }

        public bool Any(Expression<Func<Resume, bool>> predicate)
        {
            return _context.Resume.Any(predicate);
        }

        public IEnumerable<Resume> GetAll()
        {
            return _context.Resume;
        }

        public Resume GetById(string id)
        {
            return _context.Resume.Find(id);
        }

        public Resume GetByUserId(string id)
        {
            return _context.Resume.SingleOrDefault(x => x.UserId == id); ;
        }

        public int Remove(Resume resume)
        {
            _context.Resume.Remove(resume);
            return _context.SaveChanges();
        }

        public int Update(Resume resume)
        {
            _context.Resume.Update(resume);
            return _context.SaveChanges();
        }
    }
}

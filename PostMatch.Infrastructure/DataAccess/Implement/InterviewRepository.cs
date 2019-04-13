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
     public class InterviewRepository : IInterviewRepository
    {
        private readonly MyContext _context;

        public InterviewRepository(MyContext context)
        {
            _context = context;
        }

        public int Add(Interview interview)
        {
            _context.Interviews.Add(interview);
            return _context.SaveChanges();
        }

        public bool Any(Expression<Func<Interview, bool>> predicate)
        {
            return _context.Interviews.Any(predicate);
        }

        public IEnumerable<Interview> GetAll()
        {
            return _context.Interviews;
        }

        public Interview GetById(string id)
        {
            return _context.Interviews.Find(id);
        }

        public int Patch(Interview interview)
        {
            _context.Interviews.Attach(interview);
            return _context.SaveChanges();
        }

        public int Remove(Interview interview)
        {
            _context.Interviews.Remove(interview);
            return _context.SaveChanges();
        }

        public int Update(Interview interview)
        {
            _context.Interviews.Update(interview);
            return _context.SaveChanges();
        }
    }
}


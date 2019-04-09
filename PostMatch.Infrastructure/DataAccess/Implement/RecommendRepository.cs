﻿using PostMatch.Core.Entities;
using PostMatch.Infrastructure.DataAccess.Interface;
using PostMatch.Infrastructure.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PostMatch.Infrastructure.DataAccess.Implement
{
    public class RecommendRepository : IRecommendRepository
    {
        private readonly MyContext _context;

        public RecommendRepository(MyContext context)
        {
            _context = context;
        }

        public int Add(Recommend recommend)
        {
            _context.Recommend.Add(recommend);
            return _context.SaveChanges();
        }

        public bool Any(Expression<Func<Recommend, bool>> predicate)
        {
            return _context.Recommend.Any(predicate);
        }

        public IEnumerable<Recommend> GetAll()
        {
            return _context.Recommend;
        }

        public Recommend GetById(string id)
        {
            return _context.Recommend.Find(id);
        }

        public int Remove(Recommend recommend)
        {
            _context.Recommend.Remove(recommend);
            return _context.SaveChanges();
        }

        public int Update(Recommend recommend)
        {
            _context.Recommend.Update(recommend);
            return _context.SaveChanges();
        }
    }
}

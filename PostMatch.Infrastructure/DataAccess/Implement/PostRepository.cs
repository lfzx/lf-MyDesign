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
    public class PostRepository : IPostRepository
    {
        private readonly MyContext _context;

        public PostRepository(MyContext context)
        {
            _context = context;
        }

        public int Add(Post post)
        {
            _context.Post.Add(post);
            return _context.SaveChanges();
        }

        public bool Any(Expression<Func<Post, bool>> predicate)
        {
            return _context.Post.Any(predicate);
        }

        public IEnumerable<Post> GetAll()
        {
            return _context.Post;
        }

        //public Post GetByCompanyId(string id)
        //{
        //    return _context.Post.SingleOrDefault(x => x.CompanyId == id); 
        //}

        public Post GetById(string id)
        {
            return _context.Post.Find(id);
        }

        public int Remove(Post post)
        {
            _context.Post.Remove(post);
            return _context.SaveChanges();
        }

        public int Update(Post post)
        {
            _context.Post.Update(post);
            return _context.SaveChanges();
        }
    }
}

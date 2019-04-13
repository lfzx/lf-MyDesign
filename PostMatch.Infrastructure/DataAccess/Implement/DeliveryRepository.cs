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
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly MyContext _context;

        public DeliveryRepository(MyContext context)
        {
            _context = context;
        }

        public int Add(Delivery delivery)
        {
            _context.Deliveries.Add(delivery);
            return _context.SaveChanges();
        }

        public bool Any(Expression<Func<Delivery, bool>> predicate)
        {
            return _context.Deliveries.Any(predicate);
        }

        public IEnumerable<Delivery> GetAll()
        {
            return _context.Deliveries;
        }

        public Delivery GetById(string id)
        {
            return _context.Deliveries.Find(id);
        }

        public int Patch(Delivery delivery)
        {
            _context.Deliveries.Attach(delivery);
            return _context.SaveChanges();
        }

        public int Remove(Delivery delivery)
        {
            _context.Deliveries.Remove(delivery);
            return _context.SaveChanges();
        }

        public int Update(Delivery delivery)
        {
            _context.Deliveries.Update(delivery);
            return _context.SaveChanges();
        }
    }
}

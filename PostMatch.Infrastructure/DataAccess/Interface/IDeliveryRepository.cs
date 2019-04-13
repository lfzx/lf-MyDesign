using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PostMatch.Infrastructure.DataAccess.Interface
{
    public interface IDeliveryRepository
    {
        IEnumerable<Delivery> GetAll();
        Delivery GetById(string id);
        bool Any(Expression<Func<Delivery, bool>> predicate);
        int Add(Delivery delivery);
        int Update(Delivery delivery);
        int Patch(Delivery delivery);
        int Remove(Delivery delivery);
    }
}

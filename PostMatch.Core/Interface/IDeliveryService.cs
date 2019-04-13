using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Core.Interface
{
    public interface IDeliveryService
    {
        IEnumerable<Delivery> GetAll();
        Delivery GetById(string id);
        Delivery Create(Delivery delivery, string postId, string resumeId);
        void Update(Delivery delivery);
        void Patch(Delivery delivery);
        void Delete(string id);
    }
}

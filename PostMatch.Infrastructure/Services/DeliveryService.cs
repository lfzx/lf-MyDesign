using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using PostMatch.Infrastructure.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Infrastructure.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _iDeliveryRepository;
        private readonly IPostRepository _iPostRepository;
        private readonly IResumeRepository _iResumeRepository;

        public DeliveryService(
            IDeliveryRepository iDeliveryRepository,
            IPostRepository iPostRepository,
            IResumeRepository iResumeRepository
            )
        {
            _iDeliveryRepository = iDeliveryRepository;
            _iPostRepository = iPostRepository;
            _iResumeRepository = iResumeRepository;
        }

        public Delivery Create(Delivery delivery, string postId, string resumeId)
        {
            // 验证
            if (string.IsNullOrWhiteSpace(resumeId))
                throw new AppException("简历id不能为空！");
            if (string.IsNullOrWhiteSpace(postId))
                throw new AppException("职位id不能为空！");

            var post = _iPostRepository.GetById(postId);

            if (post == null)
            {
                throw new AppException("该职位不存在！");
            }

            var resume = _iResumeRepository.GetById(resumeId);

            if (resume == null)
            {
                throw new AppException("该简历不存在！");
            }

            delivery.DeliveryId = Guid.NewGuid().ToString();

            delivery.PostId = postId;
            delivery.ResumeId = resumeId;
            delivery.DeliveryUpdateTime = DateTime.Now;

            _iDeliveryRepository.Add(delivery);

            return delivery;
        }

        public void Delete(string id)
        {
            var delivery = _iDeliveryRepository.GetById(id);
            if (delivery != null)
            {
                _iDeliveryRepository.Remove(delivery);
            }
        }

        public IEnumerable<Delivery> GetAll()
        {
            return _iDeliveryRepository.GetAll();
        }

        public Delivery GetById(string id)
        {
            return _iDeliveryRepository.GetById(id);
        }

        public void Patch(Delivery delivery, string userId)
        {
            throw new NotImplementedException();
        }

        public void Update(Delivery delivery)
        {
            var deliveries = _iDeliveryRepository.GetById(delivery.DeliveryId);

            if (deliveries == null)
                throw new AppException("该投递不存在！");

            // update user properties
            deliveries.PostId = delivery.PostId;
            deliveries.ResumeId = delivery.ResumeId;
            deliveries.DeliveryUpdateTime = DateTime.Now;

            _iDeliveryRepository.Update(deliveries);
        }
    }
}

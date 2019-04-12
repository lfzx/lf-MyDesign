using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostMatch.Api.Models;
using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostMatch.Api.Controllers
{
    [Route("api/passport/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class DeliveryController : ControllerApiBase
    {
        private readonly IUserService _iUserService;
        private readonly IResumeService _iResumeService;
        private readonly ICompanyService _iCompanyService;
        private readonly IPostService _iPostService;
        private readonly IDeliveryService _iDeliveryService;
        private readonly IMapper _iMapper;

        public DeliveryController(
            IUserService iUserService,
            IResumeService iResumeService,
            ICompanyService iCompanyService,
            IPostService iPostService,
            IDeliveryService iDeliveryService,
            IMapper iMapper)
        {
            _iCompanyService = iCompanyService;
            _iPostService = iPostService;
            _iDeliveryService = iDeliveryService;
            _iUserService = iUserService;
            _iResumeService = iResumeService;
            _iMapper = iMapper;
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]DeliveryModel deliveryModel)
        {
            // map dto to entity
            var delivery = _iMapper.Map<Delivery>(deliveryModel);

            try
            {
                // save 
                var result = _iDeliveryService.Create(delivery, delivery.PostId,delivery.ResumeId);
                var count = 1;
                if (result != null)
                {
                    return Output(new DeliveryModel
                    {
                        DeliveryId = result.DeliveryId,
                        PostId = result.PostId,
                        ResumeId = result.ResumeId,
                        DeliveryUpdateTime = result.DeliveryUpdateTime
                    }, count);
                }
                throw new Exception("创建失败！");

            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var deliveries = _iDeliveryService.GetAll();
            var deliveryModels = _iMapper.Map<IList<DeliveryModel>>(deliveries);
            var count = deliveryModels.Count();
            if (deliveryModels != null)
            {

                return Output(deliveryModels, count);
            }
            throw new Exception("没有数据");

        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var delivery = _iDeliveryService.GetById(id);
            var deliveryModel = _iMapper.Map<DeliveryModels>(delivery);
           
            var post = _iPostService.GetById(deliveryModel.PostId);
            var postModel = _iMapper.Map<PostModel>(post);
            var resume = _iResumeService.GetById(deliveryModel.ResumeId);
            var resumeModel = _iMapper.Map<ResumeModel>(resume);
            var company = _iCompanyService.GetById(postModel.CompanyId);
            var companyModel = _iMapper.Map<ResponseCompanyUserModel>(company);
            var user = _iUserService.GetById(resumeModel.UserId);
            var userModel = _iMapper.Map<ResponseUserModel>(user);

            deliveryModel.postModel = postModel;
            deliveryModel.resumeModel = resumeModel;
            deliveryModel.companyUserModel = companyModel;
            deliveryModel.userModel = userModel;

            var count = 1;
            if (resumeModel != null)
            {

                return Output(
                    deliveryModel,
                     count);
            }
            throw new Exception("该投递不存在");

        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]DeliveryModel deliveryModel)
        {
            // map dto to entity and set id
            var delivery = _iMapper.Map<Delivery>(deliveryModel);
            delivery.DeliveryId = id;
            var count = 1;

            try
            {
                // save 
                _iDeliveryService.Update(delivery);
                return Output(new DeleteOrUpdateResponse
                {
                    id = id
                }, count);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var delivery = _iDeliveryService.GetById(id);
            var count = 1;
            if (delivery == null)
            {
                throw new Exception("该投递不存在");
            }
            try
            {
                // save 
                _iDeliveryService.Delete(id);
                return Output(new DeleteOrUpdateResponse
                {
                    id = id
                }, count);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

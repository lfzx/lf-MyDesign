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
    public class RecommendController : ControllerApiBase
    {
        private readonly IUserService _iUserService;
        private readonly IResumeService _iResumeService;
        private readonly ICompanyService _iCompanyService;
        private readonly IPostService _iPostService;
        private readonly IRecommendService _iRecommendService;
        private readonly IMapper _iMapper;

        public RecommendController(
            IUserService iUserService,
            IResumeService iResumeService,
            ICompanyService iCompanyService,
            IPostService iPostService,
            IRecommendService iRecommendService,
            IMapper iMapper)
        {
            _iCompanyService = iCompanyService;
            _iPostService = iPostService;
            _iRecommendService = iRecommendService;
            _iUserService = iUserService;
            _iResumeService = iResumeService;
            _iMapper = iMapper;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RecommendModel recommendModel)
        {
            // map dto to entity
            var recommend = _iMapper.Map<Recommend>(recommendModel);

            try
            {
                // save 
                var result = _iRecommendService.Create(
                    recommend,
                    recommendModel.PostId,
                    recommendModel.ResumeId);
                var count = 1;
                if (result != null)
                {
                    return Output(new RecommendModel
                    {
                        ResumeId = result.ResumeId,
                        PostId = result.PostId,
                        RecommendNumber = result.RecommendNumber,
                        RecommendId = result.RecommendId,
                        RecommendUpdateTime = result.RecommendUpdateTime
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
            var recommends = _iRecommendService.GetAll();
            var recommendModels = _iMapper.Map<IList<RecommendModel>>(recommends);
            var count = recommendModels.Count();
            if (recommendModels != null)
            {

                return Output(recommendModels, count);
            }
            throw new Exception("没有数据");

        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var recommend = _iRecommendService.GetById(id);
            var recommendModel = _iMapper.Map<RecommendModels>(recommend);
            var post = _iPostService.GetById(recommendModel.PostId);
            var postModel = _iMapper.Map<PostModel>(post);
            var resume= _iResumeService.GetById(recommendModel.ResumeId);
            var resumeModel = _iMapper.Map<ResumeModel>(resume);
            var company = _iCompanyService.GetById(post.CompanyId);
            var companyModel = _iMapper.Map<ResponseCompanyUserModel>(company);
            var user = _iUserService.GetById(resume.UserId);
            var userModel = _iMapper.Map<ResponseUserModel>(user);

            recommendModel.postModel = postModel;
            recommendModel.resumeModel = resumeModel;
            recommendModel.companyUserModel = companyModel;
            recommendModel.userModel = userModel;

            var count = 1;
            if (recommendModel != null)
            {

                return Output(
                    recommendModel,
                     count);
            }
            throw new Exception("该推荐不存在");

        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]RecommendModel recommendModel)
        {
            // map dto to entity and set id
            var recommend = _iMapper.Map<Recommend>(recommendModel);
            recommend.RecommendId = id;
            var count = 1;

            try
            {
                // save 
                _iRecommendService.Update(recommend);
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
            var recommend = _iRecommendService.GetById(id);
            var count = 1;
            if (recommend == null)
            {
                throw new Exception("该推荐不存在");
            }
            try
            {
                // save 
                _iRecommendService.Delete(id);
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

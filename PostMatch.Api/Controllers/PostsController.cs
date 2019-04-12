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
    public class PostsController : ControllerApiBase
    {
        private readonly ICompanyService _iCompanyService;
        private readonly IPostService _iPostService;
        private readonly IMapper _iMapper;

        public PostsController(
            ICompanyService iCompanyService,
            IPostService iPostService,
            IMapper iMapper)
        {
            _iCompanyService = iCompanyService;
            _iPostService = iPostService;
            _iMapper = iMapper;
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]PostModel postModel)
        {
            // map dto to entity
            var post = _iMapper.Map<Post>(postModel);

            try
            {
                // save 
                var result =_iPostService.Create(post, postModel.CompanyId);
                var count = 1;
                if (result != null)
                {
                    return Output(new DeleteOrUpdateResponse
                    {
                        id = result.PostId
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
            var posts = _iPostService.GetAll();
            var postModels = _iMapper.Map<IList<PostModel>>(posts);
            var count = postModels.Count();
            if (postModels != null)
            {

                return Output(postModels, count);
            }
            throw new Exception("没有数据");

        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var post = _iPostService.GetById(id);
            var postModel = _iMapper.Map<PostModels>(post);
            var company = _iCompanyService.GetById(postModel.CompanyId);
            var companyModel = _iMapper.Map<CompanyUserModel>(company);
            postModel.company = companyModel;
            var count = 1;
            if (postModel != null)
            {

                return Output(postModel, count);
            }
            throw new Exception("该职位不存在");

        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]PostModel postModel)
        {
            // map dto to entity and set id
            var post = _iMapper.Map<Post>(postModel);
            post.PostId= id;
            var count = 1;

            try
            {
                // save 
                _iPostService.Update(post);
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
            var post = _iPostService.GetById(id);
            var count = 1;
            if (post == null)
            {
                throw new Exception("该职位不存在");
            }
            try
            {
                // save 
                _iPostService.Delete(id);
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

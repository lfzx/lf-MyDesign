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
    public class InterviewController : ControllerApiBase
    {
        private readonly IUserService _iUserService;
        private readonly IResumeService _iResumeService;
        private readonly ICompanyService _iCompanyService;
        private readonly IPostService _iPostService;
        private readonly IInterviewService _iInterviewService;
        private readonly IMapper _iMapper;

        public InterviewController(
            IUserService iUserService,
            IResumeService iResumeService,
            ICompanyService iCompanyService,
            IPostService iPostService,
            IInterviewService iInterviewService,
            IMapper iMapper)
        {
            _iCompanyService = iCompanyService;
            _iPostService = iPostService;
            _iInterviewService = iInterviewService;
            _iUserService = iUserService;
            _iResumeService = iResumeService;
            _iMapper = iMapper;
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]InterviewModel interviewModel)
        {
            // map dto to entity
            var interview = _iMapper.Map<Interview>(interviewModel);

            try
            {
                // save 
                var result = _iInterviewService.Create(interview, interview.PostId, interview.ResumeId);
                var count = 1;
                if (result != null)
                {
                    return Output(new DeleteOrUpdateResponse
                    {
                        id = result.InterviewId,
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
            var interviews = _iInterviewService.GetAll();
            var interviewModels = _iMapper.Map<IList<InterviewModel>>(interviews);
            var count = interviewModels.Count();
            if (interviewModels != null)
            {

                return Output(interviewModels, count);
            }
            throw new Exception("没有数据");

        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var interview = _iInterviewService.GetById(id);
            var interviewModels = _iMapper.Map<InterviewModels>(interview);

            var post = _iPostService.GetById(interview.PostId);
            var postModel = _iMapper.Map<PostModel>(post);
            var resume = _iResumeService.GetById(interview.ResumeId);
            var resumeModel = _iMapper.Map<ResumeModel>(resume);
            var company = _iCompanyService.GetById(post.CompanyId);
            var companyModel = _iMapper.Map<ResponseCompanyUserModel>(company);
            var user = _iUserService.GetById(resume.UserId);
            var userModel = _iMapper.Map<ResponseUserModel>(user);

            interviewModels.postModel = postModel;
            interviewModels.resumeModel = resumeModel;
            interviewModels.companyUserModel = companyModel;
            interviewModels.userModel = userModel;

            var count = 1;
            if (resumeModel != null)
            {

                return Output(
                    interviewModels,
                     count);
            }
            throw new Exception("该面试邀请不存在");

        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]InterviewModel interviewModel)
        {
            // map dto to entity and set id
            var interview = _iMapper.Map<Interview>(interviewModel);
            interview.InterviewId = id;
            var count = 1;

            try
            {
                // save 
                _iInterviewService.Update(interview);
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
            var interview = _iInterviewService.GetById(id);
            var count = 1;
            if (interview == null)
            {
                throw new Exception("该面试邀请不存在");
            }
            try
            {
                // save 
                _iInterviewService.Delete(id);
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

        [HttpPatch("{id}")]
        public IActionResult Patch(string id, [FromBody]InterviewModel interviewModel)
        {
            // map dto to entity and set id
            var user = _iMapper.Map<Interview>(interviewModel);
            user.InterviewId = id;
            var count = 1;

            try
            {
                // save 
                _iInterviewService.Patch(user);
                return Output(new DeleteOrUpdateResponse
                {
                    id = user.InterviewId,
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

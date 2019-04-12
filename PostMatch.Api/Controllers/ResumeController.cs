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

namespace PostMatch.Api.Controllers
{
    [Route("api/passport/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ResumeController : ControllerApiBase
    {
        private readonly IUserService _iUserService;
        private readonly IResumeService _iResumeService;
        private readonly IMapper _iMapper;

        public ResumeController(
            IUserService iUserService,
            IResumeService iResumeService,
            IMapper iMapper)
        {
            _iUserService = iUserService;
            _iResumeService = iResumeService;
            _iMapper = iMapper;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]ResumeModel resumeModel)
        {
            // map dto to entity
            var resume = _iMapper.Map<Resume>(resumeModel);

            try
            {
                // save 
                var result = _iResumeService.Create(resume, resumeModel.UserId);
                var count = 1;
                if (result != null)
                {
                    return Output(new DeleteOrUpdateResponse
                    {
                        id = result.ResumeId,
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
            var resumes = _iResumeService.GetAll();
            var resumeModels = _iMapper.Map<IList<ResumeModel>>(resumes);
            var count = resumeModels.Count();
            if (resumeModels != null)
            {

                return Output(resumeModels, count);
            }
            throw new Exception("没有数据");

        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var resume = _iResumeService.GetById(id);
            var resumeModel = _iMapper.Map<ResponseResumeModel>(resume);
            var user = _iUserService.GetById(resume.UserId);
            var userModel = _iMapper.Map<ResponseUserModel>(user);
            resumeModel.responseUserModel= userModel;
            var count = 1;
            if (resumeModel != null)
            {

                return Output(resumeModel, count);
            }
            throw new Exception("该简历不存在");

        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]ResumeModel resumeModel)
        {
            // map dto to entity and set id
            var user = _iMapper.Map<Resume>(resumeModel);
            user.ResumeId = id;
            var count = 1;

            try
            {
                // save 
                _iResumeService.Update(user);
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
            var resume = _iResumeService.GetById(id);
            var count = 1;
            if (resume == null)
            {
                throw new Exception("该简历不存在");
            }
            try
            {
                // save 
                _iResumeService.Delete(id);
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
        public IActionResult Patch(string id, [FromBody]ResumeModel resumeModel)
        {
            // map dto to entity and set id
            var resume = _iMapper.Map<Resume>(resumeModel);
            resume.ResumeId = id;
            var count = 1;

            try
            {
                // save 
                _iResumeService.Patch(resume, resumeModel.UserId);
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

using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using PostMatch.Api.Helpers;
using PostMatch.Api.Models;
using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace PostMatch.Api.Controllers
{
    [Route("api/passport/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CompanyController : ControllerApiBase
    {
        private readonly ICompanyService _iCompanyService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _iMapper;
        private readonly IPostService _iPostService;

        public CompanyController(
            ICompanyService iCompanyService,
            IMapper iMapper,
            IPostService iPostService,
            IOptions<AppSettings> appSettings)
        {
            _iCompanyService = iCompanyService;
            _appSettings = appSettings.Value;
            _iMapper = iMapper;
            _iPostService = iPostService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public JsonResult Authenticate([FromBody]CompanyUserModel userModel)
        {
            var user = _iCompanyService.Authenticate(userModel.Email, userModel.Password); 

            if (user != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.CompanyId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                var count = 1;
       
                return Output(new CompanyLoginResponse
                {
                    token = tokenString,
                    avatar = user.Avatar,
                    email = user.Email,
                    name = user.CompanyName,
                    roleid = user.RoleId,
                    id = user.CompanyId,
                    OrganizationCode = user.OrganizationCode,
                    PersonalNumber = user.PersonalNumber,
                    CompanyDescription = user.CompanyDescription,
                    CompanyUrl = user.CompanyUrl,
                },count);
            }
            throw new Exception("无效用户");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]CompanyUserModel userModel)
        {
            // map dto to entity
            var user = _iMapper.Map<Companies>(userModel);
            var count = 1;

            try
            {
                // save 
                var result = _iCompanyService.Create(user, userModel.Password);
                if (result != null)
                {
                    return Output(new DeleteOrUpdateResponse
                    {
                        id = user.CompanyId,
                    },count);
                }
                throw new Exception("注册失败！");

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
            var users = _iCompanyService.GetAll();
            var userModels = _iMapper.Map<IList<CompanyUserModel>>(users);
            var count = userModels.Count();
            if (userModels != null)
            {

                return Output(userModels,count);
            }
            throw new Exception("没有数据");

        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var user = _iCompanyService.GetById(id);
            var userModel = _iMapper.Map<CompanyUserModel>(user);
            var count = 1;
            if (userModel != null)
            {

                return Output(new CompanyLoginResponse {
                    avatar = userModel.Avatar,
                    email = userModel.Email,
                    name = userModel.CompanyName,
                    id = userModel.CompanyId,
                    OrganizationCode = userModel.OrganizationCode,
                    PersonalNumber = userModel.PersonalNumber,
                    CompanyDescription = userModel.CompanyDescription,
                    CompanyUrl = userModel.CompanyUrl,
                }, count);
            }
            throw new Exception("该公司未注册");

        }

        [HttpGet("post/{id}")]
        public IActionResult GetByCompanyIdForPost(string id)
        {
            DataSet item = _iCompanyService.GetByName(id);
            var count = item.Tables[0].Rows.Count;
            if (item == null)
                return null;

            return Output(item,count);
        }

        [HttpGet("delivery/{id}")]
        public IActionResult GetByCompanyIdForDelivery(string id)
        {
            DataSet item = _iCompanyService.GetByIdForDelivery(id);
            var count = item.Tables[0].Rows.Count;
            if (item == null)
                return null;

            return Output(item, count);
        }

        [HttpGet("recommend/{id}")]
        public IActionResult GetByCompanyIdForRecommend(string id)
        {
            DataSet item = _iCompanyService.GetByIdForRecommend(id);
            var count = item.Tables[0].Rows.Count;
            if (item == null)
                return null;

            return Output(item, count);
        }

        [HttpGet("interview/{id}")]
        public IActionResult GetByCompanyIdForInterview(string id)
        {
            DataSet item = _iCompanyService.GetByIdForInterview(id);
            var count = item.Tables[0].Rows.Count;
            if (item == null)
                return null;

            return Output(item, count);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]CompanyUserModel userModel)
        {
            // map dto to entity and set id
            var user = _iMapper.Map<Companies>(userModel);
            var count = 1;
            user.CompanyId = id;

            try
            {
                // save 
                _iCompanyService.Update(user, userModel.Password);
                return Output(new DeleteOrUpdateResponse
                {
                    id = id
                },count);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(string id, [FromBody]CompanyUserModel userModel)
        {
            // map dto to entity and set id
            var user = _iMapper.Map<Companies>(userModel);
            user.CompanyId = id;
            var count = 1;

            try
            {
                // save 
                _iCompanyService.Patch(user, userModel.Password);
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

        [AllowAnonymous]
        [HttpPut("password/{id}")]
        public IActionResult EditPassword(string id, [FromBody]CompanyUserModel userModel)
        {
            // map dto to entity and set id
            var user = _iMapper.Map<Companies>(userModel);
            user.CompanyId = id;

            var result = _iCompanyService.Authenticate(userModel.Email, userModel.Password);

            if (result != null)
            {
                try
                {
                    // save 
                    _iCompanyService.EditPassword(user, userModel.NewPassword);
                    var count = 1;
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
            throw new Exception("密码或email错误，请检查");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var user = _iCompanyService.GetById(id);
            var count = 1;
            if (user == null)
            {
                throw new Exception("该公司未注册");
            }
            try
            {
                // save 
                _iCompanyService.Delete(id);
                return Output(new DeleteOrUpdateResponse
                {
                    id = id
                },count);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}

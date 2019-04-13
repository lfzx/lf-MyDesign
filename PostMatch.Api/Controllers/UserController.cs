using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostMatch.Api.Helpers;
using PostMatch.Api.Models;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using User = PostMatch.Core.Entities.User;

namespace PostMatch.Api.Controllers
{
    [Route("api/passport")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerApiBase
    {
        private readonly IUserService _iUserService;
        private readonly IResumeService _iResumeService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _iMapper;

        public UserController(
            IUserService iUserService,
            IResumeService iResumeService,
            IMapper iMapper,
            IOptions<AppSettings> appSettings)
        {
            _iUserService = iUserService;
            _iResumeService = iResumeService;
            _appSettings = appSettings.Value;
            _iMapper = iMapper;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public JsonResult Authenticate([FromBody]UserModel userModel)
        {
                var user = _iUserService.Authenticate(userModel.Email, userModel.Password);

                if (user != null)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);
                    var count = 1;
                    var resumeid = "";
                    var resume = _iResumeService.GetByUserId(user.Id);
                    if (resume != null)
                    {
                    resumeid = resume.ResumeId;
                    }

                return Output(new UserLoginResponse
                    {
                        token = tokenString,
                        roleid = user.RoleId,
                        resumeid = resumeid,
                        avatar = user.Avatar,
                        email = user.Email,
                        name = user.Name,
                        id = user.Id,
                        school = user.School,
                        gender = user.Gender,
                        entranceTime = user.EntranceTime,
                        graduationTime = user.GraduationTime,
                        profession = user.Profession,
                        academic = user.Academic
                    },count);
                }
                throw new Exception("无效用户");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserModel userModel)
        {
            // map dto to entity
            var user = _iMapper.Map<User>(userModel);

            try
            {
                // save 
                var result = _iUserService.Create(user, userModel.Password);
                var count = 1;
                if (result != null)
                {
                    return Output(new DeleteOrUpdateResponse
                    {
                        id = user.Id,
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
            var users = _iUserService.GetAll();
            var userModels = _iMapper.Map<IList<UserModel>>(users);
            var count = userModels.Count();
            if(userModels != null)
            {

                return Output(userModels,count);
            }
            throw new Exception("没有数据");
        
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var user = _iUserService.GetById(id);
            var userModel = _iMapper.Map<UserModel>(user);
            var count = 1;
            if (userModel != null)
            {

                return Output(userModel,count);
            }
            throw new Exception("该用户不存在");

        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]UserModel userModel)
        {
            // map dto to entity and set id
            var user = _iMapper.Map<User>(userModel);
            user.Id = id;
            var count = 1;

            try
            {
                // save 
                 _iUserService.Update(user, userModel.Password);
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

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var user = _iUserService.GetById(id);
            var count = 1;
            if(user == null)
            {
                throw new Exception("该用户不存在");
            }
            try
            {
                // save 
                _iUserService.Delete(id);
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
        public IActionResult Patch(string id, [FromBody]UserModel userModel)
        {
            // map dto to entity and set id
            var user = _iMapper.Map<User>(userModel);
            user.Id = id;
            var count = 1;

            try
            {
                // save 
                _iUserService.Patch(user, userModel.Password);
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

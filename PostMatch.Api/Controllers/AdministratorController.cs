using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostMatch.Api.Helpers;
using PostMatch.Api.Models;
using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PostMatch.Api.Controllers
{
    [Route("api/passport/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AdministratorController : ControllerApiBase
    {
        private readonly IAdministratorService _iAdministratorService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _iMapper;

        public AdministratorController(
            IAdministratorService iAdministratorService,
            IMapper iMapper,
            IOptions<AppSettings> appSettings)
        {
            _iAdministratorService = iAdministratorService;
            _appSettings = appSettings.Value;
            _iMapper = iMapper;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public JsonResult Authenticate([FromBody]AdministratorModel userModel)
        {
            var user = _iAdministratorService.Authenticate(userModel.Email, userModel.Password);
            var count = 1;

            if (user.Status != 0)
            {
                if (user != null)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                    new Claim(ClaimTypes.Name, user.AdminId.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    return Output(new LoginResponse
                    {
                        token = tokenString,
                        avatar = user.Avatar,
                        email = user.Email,
                        name = user.AdminName,
                        roleid = user.RoleId,
                        school = user.School,
                        id = user.AdminId
                    }, count);
                }
                throw new Exception("用户名或密码错误！");
            }
            throw new Exception("还未通过审核！请稍后再试！");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]AdministratorModel userModel)
        {
            // map dto to entity
            var user = _iMapper.Map<Administrator>(userModel);
            var count = 1;

            try
            {
                // save 
                var result = _iAdministratorService.Create(user, userModel.Password);
                if (result != null)
                {
                    return Output(new LoginResponse
                    {
                        email = user.Email,
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
            var users = _iAdministratorService.GetAll();
            var userModels = _iMapper.Map<IList<AdministratorModel>>(users);
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
            var user = _iAdministratorService.GetById(id);
            var userModel = _iMapper.Map<AdministratorModel>(user);
            var count = 1;
            if (userModel != null)
            {

                return Output(new UserModel
                {
                    
                    Avatar = userModel.Avatar,
                    Email = userModel.Email,
                    Name = userModel.AdminName,
                    School = userModel.School,
                    Id= userModel.AdminId
                }, count);
            }
            throw new Exception("该用户不存在");

        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]AdministratorModel userModel)
        {
            // map dto to entity and set id
            var user = _iMapper.Map<Administrator>(userModel);
            var count = 1;
            user.AdminId = id;

            try
            {
                // save 
                _iAdministratorService.Update(user, userModel.Password);
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
            var user = _iAdministratorService.GetById(id);
            var count = 1;
            if (user == null)
            {
                throw new Exception("该用户不存在");
            }
            try
            {
                // save 
                _iAdministratorService.Delete(id);
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

        [AllowAnonymous]
        [HttpPut("password/{id}")]
        public IActionResult EditPassword(string id, [FromBody]AdministratorModel administratorModel)
        {
            // map dto to entity and set id
            var user = _iMapper.Map<Administrator>(administratorModel);
            user.AdminId = id;

            var result = _iAdministratorService.Authenticate(administratorModel.Email, administratorModel.Password);

            if (result != null)
            {
                try
                {
                    // save 
                    _iAdministratorService.EditPassword(user, administratorModel.NewPassword);
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
    }
}

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
                    id = user.AdminId
                });
            }
            throw new Exception("无效用户");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]AdministratorModel userModel)
        {
            // map dto to entity
            var user = _iMapper.Map<Administrator>(userModel);

            try
            {
                // save 
                var result = _iAdministratorService.Create(user, userModel.Password);
                if (result != null)
                {
                    return Output(new LoginResponse
                    {
                        email = user.Email,
                    });
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
            if (userModels != null)
            {

                return Output(userModels);
            }
            throw new Exception("没有数据");

        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var user = _iAdministratorService.GetById(id);
            var userModel = _iMapper.Map<AdministratorModel>(user);
            if (userModel != null)
            {

                return Output(userModel);
            }
            throw new Exception("该用户不存在");

        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]AdministratorModel userModel)
        {
            // map dto to entity and set id
            var user = _iMapper.Map<Administrator>(userModel);
            user.AdminId = id;

            try
            {
                // save 
                _iAdministratorService.Update(user, userModel.Password);
                return Output(new DeleteOrUpdateResponse
                {
                    id = id
                });
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
                });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

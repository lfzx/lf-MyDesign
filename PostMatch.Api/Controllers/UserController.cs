using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostMatch.Api.Helpers;
using PostMatch.Api.Models;
using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Models;
using PostMatch.Core.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using User = PostMatch.Core.Entities.User;

namespace PostMatch.Api.Controllers
{
    [Route("api/passport")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerApiBase
    {
        private readonly IUserService _iUserService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _iMapper;

        public UserController(IUserService iUserService, IMapper iMapper, IOptions<AppSettings> appSettings)
        {
            _iUserService = iUserService;
            _appSettings = appSettings.Value;
            _iMapper = iMapper;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Authenticate([FromBody]UserModel userModel)
        {
            var user = _iUserService.Authenticate(userModel.Email, userModel.Password);

            //if (user == null)
            //    return BadRequest(new { message = "UserEmail or password is incorrect" });

            if(user != null)
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

                return Output(new LoginResponse
            {
                firstname = user.FirstName,
                avatar = user.Avatar,
                email = user.Email,
                username = user.UserName,
                token = tokenString
            });
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
                _iUserService.Create(user, userModel.Password);
                return Ok();
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
            return Ok(userModels);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var user = _iUserService.GetById(id);
            var userModel = _iMapper.Map<UserModel>(user);
            return Ok(userModel);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]UserModel userModel)
        {
            // map dto to entity and set id
            var user = _iMapper.Map<User>(userModel);
            user.Id = id;

            try
            {
                // save 
                _iUserService.Update(user, userModel.Password);
                return Ok();
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
            _iUserService.Delete(id);
            return Ok();
        }
    }
}

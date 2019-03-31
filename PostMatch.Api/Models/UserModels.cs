using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PostMatch.Api.Models
{
    public class User
    {
        public string id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string email { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        public string password { get; set; }
    }

    public class LoginResponse
    {
        public string token { get; set; }

        public string username { get; set; }

        public string firstname { get; set; }

        public string email { get; set; }

        public string avatar { get; set; }
    }
}

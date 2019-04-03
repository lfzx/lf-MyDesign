using System.ComponentModel.DataAnnotations;

namespace PostMatch.Api.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
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

        public string name { get; set; }

        public string roleid { get; set; }

        public string id { get; set; }

        public string email { get; set; }

        public string avatar { get; set; }
    }

    public class DeleteOrUpdateResponse
    {
        public string id { get; set; }
    }

    public class CompanyUserModel
    {
        public string CompanyId { get; set; }
        public string Avatar { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string OrganizationCode { get; set; }
        public string PersonalNumber { get; set; }
        public string CompanyDescription { get; set; }
        public string CompanyUrl { get; set; }
        public string Password { get; set; }
    }

    public class AdministratorModel
    {
        public string AdminId { get; set; }
        public string Avatar { get; set; }
        public string AdminName { get; set; }
        public string Email { get; set; }
        public string School { get; set; }
        public int Status { get; set; }
        public string Password { get; set; }
    }

}


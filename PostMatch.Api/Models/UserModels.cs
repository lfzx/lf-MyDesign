using System;
using System.ComponentModel.DataAnnotations;

namespace PostMatch.Api.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public int RoleId { get; set; }
        public int IsEnable { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime UpdateTime { get; set; }
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

        public int roleid { get; set; }

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
        public int roleid { get; set; }
        public string CompanyUrl { get; set; }
        public int Status { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Password { get; set; }
    }

    public class AdministratorModel
    {
        public string AdminId { get; set; }
        public string Avatar { get; set; }
        public string AdminName { get; set; }
        public string Email { get; set; }
        public string School { get; set; }
        public int roleid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Password { get; set; }
    }

    public class ResumeModel
    {
        public string ResumeId { get; set; }
        public string UserId { get; set; }
        public string ResumeAvatar { get; set; }
        public int Gender { get; set; }
        public string ResumeTelephoneNumber { get; set; }
        public string FamilyAddress { get; set; }
        public string ResumePostName { get; set; }
        public string ResumeSalary { get; set; }
        public string ResumeWorkPlace { get; set; }
        public string ResumeJobType { get; set; }
        public string Academic { get; set; }
        public string ResumeExperience { get; set; }
        public string Skill { get; set; }
        public int IsEnable { get; set; }
        public DateTime ResumeUpdateTime { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class PostModel
    {
        public string PostId { get; set; }
        public string CompanyId { get; set; }
        public string PostDescription { get; set; }
        public string City { get; set; }
        public string PostTelephoneNumber { get; set; }
        public string NumberOfRecruits { get; set; }
        public string PostName { get; set; }
        public string PostSalary { get; set; }
        public string PostWorkPlace { get; set; }
        public string PostJobType { get; set; }
        public string AcademicRequirements { get; set; }
        public string PostExperience { get; set; }
        public string PostEmail { get; set; }
        public int IsEnable { get; set; }
        public DateTime PostUpdateTime { get; set; }
    }

    public class PostModels
    {
        public string PostId { get; set; }
        public string CompanyId { get; set; }
        public string PostDescription { get; set; }
        public string City { get; set; }
        public string PostTelephoneNumber { get; set; }
        public string NumberOfRecruits { get; set; }
        public string PostName { get; set; }
        public string PostSalary { get; set; }
        public string PostWorkPlace { get; set; }
        public string PostJobType { get; set; }
        public string AcademicRequirements { get; set; }
        public string PostExperience { get; set; }
        public string PostEmail { get; set; }
        public int IsEnable { get; set; }
        public DateTime PostUpdateTime { get; set; }

        public CompanyUserModel company = new CompanyUserModel();
    }

    public class RecommendModel
    {
        public string RecommendId { get; set; }
        public string ResumeId { get; set; }
        public string PostId { get; set; }
        public string RecommendNumber { get; set; }
        public DateTime RecommendUpdateTime { get; set; }
    }

    public class RecommendModels
    {
        public string RecommendId { get; set; }
        public string PostId { get; set; }
        public string ResumeId { get; set; }
        public string RecommendNumber { get; set; }
        public DateTime UpdateTime { get; set; }
        
        public PostModel postModel = new PostModel();
        public ResumeModel resumeModel = new ResumeModel();
        public UserModel userModel = new UserModel();
        public CompanyUserModel companyUserModel = new CompanyUserModel();
    }

    public class DeliveryModel
    {
        public string DeliveryId { get; set; }
        public string PostId { get; set; }
        public string ResumeId { get; set; }
        public DateTime DeliveryUpdateTime { get; set; }
    }

    public class DeliveryModels
    {
        public string DeliveryId { get; set; }
        public string PostId { get; set; }
        public string ResumeId { get; set; }
        public DateTime DeliveryUpdateTime { get; set; }

        public PostModel postModel = new PostModel();
        public ResumeModel resumeModel = new ResumeModel();
        public UserModel userModel = new UserModel();
        public CompanyUserModel companyUserModel = new CompanyUserModel();
    }

}


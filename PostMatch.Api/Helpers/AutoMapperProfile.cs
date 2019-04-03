using AutoMapper;
using PostMatch.Api.Models;
using PostMatch.Core.Entities;

namespace PostMatch.Api.Helpers
{

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<UserModel, User>();
            CreateMap<Companies, CompanyUserModel>();
            CreateMap<CompanyUserModel, Companies>();
            CreateMap<Administrator, AdministratorModel>();
            CreateMap<AdministratorModel, Administrator>();
        }
    }
}

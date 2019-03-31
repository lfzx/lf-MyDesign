using AutoMapper;
using PostMatch.Core.Entities;
using PostMatch.Core.Models;


namespace PostMatch.Api.Helpers
{

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<UserModel, User>();
        }
    }
}

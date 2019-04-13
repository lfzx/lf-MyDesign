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
            CreateMap<Resume, ResumeModel>();
            CreateMap<ResumeModel, Resume>();
            CreateMap<Post, PostModel>();
            CreateMap<PostModel, Post>();
            CreateMap<Recommend, RecommendModel>();
            CreateMap<RecommendModel, Recommend>();
            CreateMap<Recommend, RecommendModels>();
            CreateMap<RecommendModels, Recommend>();
            CreateMap<Delivery, DeliveryModel>();
            CreateMap<DeliveryModel, Delivery>();
            CreateMap<Delivery, DeliveryModels>();
            CreateMap<DeliveryModels, Delivery>();
            CreateMap<ResponseResumeModel, Resume>();
            CreateMap<Resume, ResponseResumeModel>();
            CreateMap<PostModels, Post>();
            CreateMap<Post, PostModels>();
            CreateMap<CompanyUserModel, Companies>();
            CreateMap<Companies, CompanyUserModel>();
            CreateMap<Interview, InterviewModel>();
            CreateMap<InterviewModel, Interview>();
            CreateMap<Interview, InterviewModels>();
            CreateMap<InterviewModels, InterviewModel>();

        }
    }
}

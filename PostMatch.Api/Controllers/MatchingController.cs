using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.DataView;
using Microsoft.ML;
using PostMatch.Api.Helpers;
using PostMatch.Api.Models;
using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using Resume = PostMatch.Api.Models.Resume;

namespace PostMatch.Api.Controllers
{
    //根据简历对其岗位进行推荐

    [Route("api/passport/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MatchingController : ControllerApiBase
    {
        ProductRepository productRepository = new ProductRepository();//直接数据库操作
        Calculate calculate = new Calculate();//直接匹配操作

        private readonly IUserService _iUserService;
        private readonly IResumeService _resumeService;
        private readonly IPostService _iPostService;
        private readonly IRecommendService _iRecommendService;
        List<Post> list = new List<Post>();
        List<Recommend> finalResult = new List<Recommend>();

        //List<Resume> lists = new List<Resume>();

        public MatchingController(
             IUserService iUserService,
             IResumeService resumeService,
             IPostService iPostService,
            IRecommendService iRecommendService)
        {
            _iUserService = iUserService;
            _resumeService = resumeService;
            _iPostService = iPostService;
            _iRecommendService = iRecommendService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<string> Post([FromBody]Resume input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

                DataSet allPosts = productRepository.GetPosts();

                list = DataSetToList<Post>(allPosts, 0);

            //DataSet allResumes = productRepository.GetResumes();
            //lists = DataSetToList<Resume>(allResumes, 0);

            //     foreach (var resume in lists)
            //     {
            //         finalResult = calculate.GetMatchingResultsByResume(resume, list);

            //         Console.WriteLine("简历信息：期望职位名：{0},期望薪资：{1},工作经验年限：{2}，技能：{3}", resume.resumePostName,
            //resume.resumeSalary,
            //resume.workYear, resume.skill);
            //         foreach (var recommend in finalResult)
            //         {
            //             var post = _iPostService.GetById(recommend.PostId);

            //             Console.WriteLine("职位信息：职位名：{0},薪资：{1},工作经验年限要求：{2}，职位描述：{3}", post.PostName, post.PostSalary,
            //                 post.PostExperience, post.PostDescription);
            //             Console.WriteLine("----------------");

            //         }
            //     }

            //foreach (var post in list)
            //{
            //    finalResult = calculate.GetMatchingResultsByPost(lists, post);

            //    Console.WriteLine("职位信息：职位名：{0},薪资：{1},工作经验年限要求：{2}，职位描述：{3}", post.PostName, post.PostSalary,
            //         post.PostExperience, post.PostDescription);
            //    foreach (var recommend in finalResult)
            //    {
            //        Console.WriteLine("----------------");
            //   var resume = _resumeService.GetById(recommend.ResumeId);
            //        Console.WriteLine("简历信息：期望职位名：{0},期望薪资：{1},工作经验年限：{2}，技能：{3}", resume.ResumePostName,
            //            resume.ResumeSalary,
            //            resume.WorkYear, resume.Skill);
            //    }
            //}



            finalResult = calculate.GetMatchingResultsByResume(input, list);

            foreach (var recommend in finalResult)
            {
                Recommend recommends = new Recommend
                {
                    ResumeId = recommend.ResumeId,
                    PostId = recommend.PostId,
                    CompanyId = recommend.CompanyId,
                    RecommendNumber = recommend.RecommendNumber
                };
                _iRecommendService.Create(recommends, recommends.PostId, recommends.ResumeId);

            }

            return Output("ok", 5);
        }

        public List<Post> DataSetToList<Post>(DataSet dataSet, int tableIndex)
        {
            //确认参数有效
            if (dataSet == null || dataSet.Tables.Count <= 0 || tableIndex < 0)
                return null;

            DataTable dt = dataSet.Tables[tableIndex];

            List<Post> list = new List<Post>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //创建泛型对象
                Post _t = Activator.CreateInstance<Post>();
                //获取对象所有属性
                PropertyInfo[] propertyInfo = _t.GetType().GetProperties();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    foreach (PropertyInfo info in propertyInfo)
                    {
                        //属性名称和列名相同时赋值
                        if (dt.Columns[j].ColumnName.ToUpper().Equals(info.Name.ToUpper()))
                        {
                            if (dt.Rows[i][j] != DBNull.Value)
                            {
                                info.SetValue(_t, dt.Rows[i][j], null);
                            }
                            else
                            {
                                info.SetValue(_t, null, null);
                            }
                            break;
                        }
                    }
                }
                list.Add(_t);
            }
            return list;
        }

    }
}
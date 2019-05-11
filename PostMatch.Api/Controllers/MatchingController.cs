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
        private readonly IRecommendService _iRecommendService;
        List<Post> list = new List<Post>();
        List<Recommend> finalResult = new List<Recommend>();

        public MatchingController(
             IUserService iUserService,
            IRecommendService iRecommendService)
        {
            _iUserService = iUserService;
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

            DataSet finalInput = _iUserService.GetByIdForMatch(input.userId);

            foreach (DataRow ds in finalInput.Tables[0].Rows)
            {
                Resume finalResume = new Resume
                {
                    resumeId = ds[0].ToString(),
                    userId = ds[1].ToString(),
                    familyAddress = ds[2].ToString(),
                    resumePostName = ds[3].ToString(),
                    resumeSalary = ds[4].ToString(),
                    resumeWorkPlace = ds[5].ToString(),
                    resumeJobType = ds[6].ToString(),
                    resumeExperience = ds[7].ToString(),
                    skill = ds[8].ToString(),
                    birth = ds[9].ToString(),
                    workYear = ds[10].ToString(),
                    profession = ds[11].ToString(),
                    academic = ds[12].ToString()
                };

                DataSet allPosts = productRepository.GetPosts();

                list = DataSetToList<Post>(allPosts, 0);

                finalResult = calculate.GetMatchingResultsByResume(finalResume, list);

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
            }

            return Output("ok", 1);
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
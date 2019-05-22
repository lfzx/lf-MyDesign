using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using PostMatch.Api.Helpers;
using PostMatch.Core.Entities;
using PostMatch.Core.Interface;
using Resume = PostMatch.Api.Models.Resume;

namespace PostMatch.Api.Controllers
{
    [Route("api/passport/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MatchingResumeController : ControllerApiBase
    {
	    ProductRepository productRepository = new ProductRepository();//直接数据库操作
        Calculate calculate = new Calculate();//直接匹配操作
		
		List<Resume> list = new List<Resume>();
        List<Recommend> finalResult = new List<Recommend>();
	
        private readonly IPostService _iPostService;
        private readonly IRecommendService _iRecommendService;

        public MatchingResumeController(
            IPostService iPostService,
            IRecommendService iRecommendService)
        {
            _iPostService = iPostService;
            _iRecommendService = iRecommendService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<string> Resume([FromBody]Post input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
			
//			var post = _iPostService.GetById(input.PostId);

			DataSet allResumes = productRepository.GetResumes();
			list = DataSetToList<Resume>(allResumes, 0);
            finalResult = calculate.GetMatchingResultsByPost(list,input);
            
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
				return Output("ok", 1);
            }
	
	public List<Resume> DataSetToList<Resume>(DataSet dataSet, int tableIndex)
        {
            //确认参数有效
            if (dataSet == null || dataSet.Tables.Count <= 0 || tableIndex < 0)
                return null;

            DataTable dt = dataSet.Tables[tableIndex];

            List<Resume> list = new List<Resume>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //创建泛型对象
                Resume _t = Activator.CreateInstance<Resume>();
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
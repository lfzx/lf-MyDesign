using System;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using PostMatch.Api.Models;
using PostMatch.Core.Interface;

namespace PostMatch.Api.Controllers
{
    [Route("api/passport/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MatchingResumeController : ControllerApiBase
    {
        private readonly PredictionEngine<PostMatching, PostMatchingPrediction> _predictionEngine;
        private readonly IPostService _iPostService;
        private readonly IResumeService _iResumeService;
        private readonly IRecommendService _iRecommendService;

        public MatchingResumeController(PredictionEngine<PostMatching, PostMatchingPrediction> predictionEngine,
            IPostService iPostService,
             IResumeService iResumeService,
            IRecommendService iRecommendService)
        {
            _predictionEngine = predictionEngine;
            _iPostService = iPostService;
            _iResumeService = iResumeService;
            _iRecommendService = iRecommendService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<string> Post([FromBody]PostMatching input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            PostMatchingPrediction prediction = _predictionEngine.Predict(input);

            var resumeId = prediction.recommendResumeId;

            var resume = _iResumeService.GetById(resumeId);

            PostMatch.Core.Entities.Post post = new PostMatch.Core.Entities.Post
            {
               PostId = input.postId,
               RecommendResumeId = resumeId,
               CompanyId = input.companyId
            };

            _iPostService.Patch(post, input.companyId);

            PostMatch.Core.Entities.Recommend recommend = new PostMatch.Core.Entities.Recommend
            {
                ResumeId = resumeId,
                RecommendNumber = "10",
            };
            _iRecommendService.CreateForMatch(recommend, input.postId, input.companyId);

            string[] postName = resume.ResumePostName.Split('、');
            var i = 9;

            foreach (string name in postName)
            {
                var finalName = "%" + name + "%";
                DataSet item = _iResumeService.GetByName(finalName);
                var count = item.Tables[0].Rows.Count;

                var sum = count > 4 ? count / (count/2) : count;

                foreach (DataRow dr in item.Tables[0].Rows)
                {
                    if (dr[0].ToString() == resumeId)
                    {
                        continue;
                    }
                    PostMatch.Core.Entities.Recommend recommends = new PostMatch.Core.Entities.Recommend
                    {
                        ResumeId = dr[0].ToString(),
                        RecommendNumber = i.ToString(),
                    };
                    Console.WriteLine("--------" + dr[0].ToString() + "---------");
                    var result = _iRecommendService.CreateForMatch(recommends, input.postId, input.companyId);
                    i--;
                    sum--;
                    if(sum == 0)
                    {
                        break;
                    }
                    if (i == 0)
                    {
                        break;
                    }
                }
                if(i == 0)
                {
                    break;
                }
            }

            return Output(resumeId, 1);

        }  
    }
}
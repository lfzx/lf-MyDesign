using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.DataView;
using Microsoft.ML;
using PostMatch.Api.Models;
using PostMatch.Core.Entities;
using PostMatch.Core.Helpers;
using PostMatch.Core.Interface;
using Resume = PostMatch.Api.Models.Resume;

namespace PostMatch.Api.Controllers
{
    [Route("api/passport/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MatchingController : ControllerApiBase
    {
        private readonly PredictionEngine<Resume, ResumePrediction> _predictionEngine;
        private readonly IPostService _iPostService;
        private readonly IResumeService _iResumeService;
        private readonly IRecommendService _iRecommendService;

        public MatchingController(PredictionEngine<Resume, ResumePrediction> predictionEngine,
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
        public ActionResult<string> Post([FromBody]Resume input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ResumePrediction prediction = _predictionEngine.Predict(input);

            var postId = prediction.recommendPostId;

            postId = postId.Replace("\"", "");

            var post = _iPostService.GetById(postId);

            PostMatch.Core.Entities.Resume resumes = new PostMatch.Core.Entities.Resume
            {
                UserId = input.userId,
                ResumeId = input.resumeId,
                RecommendPostId = postId
            };
             _iResumeService.Patch(resumes,input.userId);

            var postName = "%" + post.PostName + "%";
            DataSet item = _iPostService.GetByName(postName);

            var count = item.Tables[0].Rows.Count;
            var i = 8;
            foreach (DataRow dr in item.Tables[0].Rows)
            {
                Recommend recommend = new Recommend
                {
                    ResumeId = input.resumeId,
                    RecommendNumber = i.ToString(),
                };
                Console.WriteLine("--------"+ dr[0].ToString(), dr[1].ToString() + "---------");
                Console.WriteLine("--------"+ dr[1].ToString() + "---------");
                var result = _iRecommendService.CreateForMatch(recommend, dr[0].ToString(), dr[1].ToString());
                i--;
            }


            if (item == null)
                return null;

            return Output(item, count);
        }

    }
}
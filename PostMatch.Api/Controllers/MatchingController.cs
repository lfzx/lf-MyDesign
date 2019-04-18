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
using PostMatch.Core.Interface;

namespace PostMatch.Api.Controllers
{
    [Route("api/passport/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MatchingController : ControllerApiBase
    {
        private readonly PredictionEngine<Resume, ResumePrediction> _predictionEngine;
        private readonly IPostService _iPostService;

        public MatchingController(PredictionEngine<Resume, ResumePrediction> predictionEngine,
            IPostService iPostService)
        {
            _predictionEngine = predictionEngine;
            _iPostService = iPostService;
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

            DataSet item = _iPostService.GetByName(post.PostName);
            var count = item.Tables[0].Rows.Count;

            if (item == null)
                return null;

            return Output(item, count);
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using PostMatch.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostMatch.Api.Helpers
{
    public class Matching
    {
        private readonly PredictionEngine<Resume, ResumePrediction> _predictionEngine;

        public Matching(PredictionEngine<Resume, ResumePrediction> predictionEngine)
        {
            _predictionEngine = predictionEngine;
        }

        public ActionResult<string> Match([FromBody]Resume resume)
        {
            ResumePrediction prediction = _predictionEngine.Predict(resume);

            //返回模型。
            return prediction.recommendPostId;
        }
    }
}

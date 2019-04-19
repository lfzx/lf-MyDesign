using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostMatch.Api.Models
{
    public class PostMatching
    {
        [LoadColumn(0)]
        public string postId { get; set; }
        [LoadColumn(1)]
        public string companyId { get; set; }
        [LoadColumn(2)]
        public string postName { get; set; }
        [LoadColumn(3)]
        public string postWorkPlace { get; set; }
        [LoadColumn(4)]
        public string postSalary { get; set; }
        [LoadColumn(5)]
        public string postJobType { get; set; }
        [LoadColumn(6)]
        public string postDescription { get; set; }
        [LoadColumn(7)]
        public string city { get; set; }
        [LoadColumn(8)]
        public string postExperience { get; set; }
        [LoadColumn(9)]
        public string academicRequirements { get; set; }
        [LoadColumn(10)]
        public string numberOfRecruits { get; set; }
        [LoadColumn(11)]
        public DateTime postUpdateTime { get; set; }
        [LoadColumn(12)]
        public string postTelephoneNumber { get; set; }
        [LoadColumn(13)]
        public string recommendResumeId { get; set; }
    }

    public class PostMatchingPrediction
    {
        [ColumnName("PredictedLabel")]
        public string recommendResumeId;
    }
}

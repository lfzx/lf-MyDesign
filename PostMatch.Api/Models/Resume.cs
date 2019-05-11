using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostMatch.Api.Models
{
    public class Resume
    {
        [LoadColumn(0)]
        public string resumeId { get; set; }
        [LoadColumn(1)]
        public string userId { get; set; }
        [LoadColumn(2)]
        public string familyAddress { get; set; }
        [LoadColumn(3)]
        public string resumePostName { get; set; }
        [LoadColumn(4)]
        public string resumeSalary { get; set; }
        [LoadColumn(5)]
        public string resumeWorkPlace { get; set; }
        [LoadColumn(6)]
        public string resumeJobType { get; set; }
        [LoadColumn(7)]
        public string resumeExperience { get; set; }
        [LoadColumn(8)]
        public string skill { get; set; }
        [LoadColumn(9)]
        public string birth { get; set; }
        [LoadColumn(10)]
        public string workYear { get; set; }
        [LoadColumn(11)]
        public string profession { get; set; }
        [LoadColumn(12)]
        public string academic { get; set; }

    }

    public class ResumePrediction
    {
        public string recommendPostId;

        public string score;
    }
}

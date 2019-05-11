using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Core.Entities
{
    public class Recommend
    {
        public string RecommendId { get; set; }
        public string PostId { get; set; }
        public string ResumeId { get; set; }
        public double RecommendNumber { get; set; }
        public DateTime RecommendUpdateTime { get; set; }
        public string Score { get; set; }
        public string CompanyScore { get; set; }
        public string CompanyId { get; set; }
    }
}

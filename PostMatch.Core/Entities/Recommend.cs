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
        public string RecommendNumber { get; set; }
        public DateTime RecommendUpdateTime { get; set; }
    }
}

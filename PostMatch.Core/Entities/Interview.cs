using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Core.Entities
{
    public class Interview
    {
        public string InterviewId { get; set; }
        public string ResumeId { get; set; }
        public string PostId { get; set; }
        public DateTime InterviewUpdateTime { get; set; }
        public DateTime InterviewTime { get; set; }
        public string CompanyId { get; set; }
        public string Place { get; set; }
        public string Note { get; set; }
        public int UserResponse { get; set; }
    }
}

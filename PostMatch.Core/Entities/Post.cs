using System;

namespace PostMatch.Core.Entities
{
    public class Post
    {
        public string PostId { get; set; }
        public string CompanyId { get; set; }
        public string PostName { get; set; }
        public string PostDescription { get; set; }
        public string City { get; set; }
        public string PostTelephoneNumber { get; set; }
        public string NumberOfRecruits { get; set; }
        public string PostSalary { get; set; }
        public string PostWorkPlace { get; set; }
        public string PostJobType { get; set; }
        public string AcademicRequirements { get; set; }
        public string PostExperience { get; set; }
        public DateTime PostUpdateTime { get; set; }

    }
}

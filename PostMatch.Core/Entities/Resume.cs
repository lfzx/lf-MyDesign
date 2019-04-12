using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostMatch.Core.Entities
{
    [Table("resume")]
    public class Resume
    {
        [Column("resumeId")]
        [Key]
        public string ResumeId { get; set; }

        [Column("userId")]
        public string UserId { get; set; }

        [Column("resumeAvatar")]
        public string ResumeAvatar { get; set; }

        [Column("resumeTelephoneNumber")]
        public string ResumeTelephoneNumber { get; set; }

        [Column("familyAddress")]
        public string FamilyAddress { get; set; }

        [Column("resumePostName")]
        public string ResumePostName { get; set; }

        [Column("resumeSalary")]
        public string ResumeSalary { get; set; }

        [Column("resumeWorkPlace")]
        public string ResumeWorkPlace { get; set; }

        [Column("resumeJobType")]
        public string ResumeJobType { get; set; }

        [Column("resumeExperience")]
        public string ResumeExperience { get; set; }

        [Column("skill")]
        public string Skill { get; set; }

        [Column("birth")]
        public string Birth { get; set; }

        [Column("isEnable")]
        public int IsEnable { get; set; }

        [Column("resumeUpdateTime")]
        public DateTime ResumeUpdateTime { get; set; }
    }
}

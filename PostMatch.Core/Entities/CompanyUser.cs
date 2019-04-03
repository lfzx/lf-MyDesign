using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostMatch.Core.Entities
{
    [Table("companies")]
    public class Companies
    {
        [Column("companyId")]
        [Key]
        public string CompanyId { get; set; }

        [Column("companyName")]
        public string CompanyName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("companyDescription")]
        public string CompanyDescription { get; set; }

        [Column("companyUrl")]
        public string CompanyUrl { get; set; }

        [Column("avatar")]
        public string Avatar { get; set; }

        [Column("organizationCode")]
        public string OrganizationCode { get; set; }

        [Column("personalNumber")]
        public string PersonalNumber { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("passwordHash")]
        public byte[] PasswordHash { get; set; }

        [Column("passwordSalt")]
        public byte[] PasswordSalt { get; set; }

    }
}

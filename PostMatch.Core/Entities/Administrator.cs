using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PostMatch.Core.Entities
{
    [Table("administrator")]
    public class Administrator
    {
        [Column("adminId")]
        [Key]
        public string AdminId { get; set; }

        [Column("avatar")]
        public string Avatar { get; set; }

        [Column("adminName")]
        public string AdminName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("school")]
        public string School { get; set; }

        [Column("roleId")]
        public int RoleId { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("updateTime")]
        public DateTime UpdateTime { get; set; }

        [Column("passwordHash")]
        public byte[] PasswordHash { get; set; }

        [Column("passwordSalt")]
        public byte[] PasswordSalt { get; set; }
    }
}

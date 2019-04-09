using System;

namespace PostMatch.Core.Entities
{
    public class User
    {
        public string Id { get; set; }
        public int RoleId { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int IsEnable { get; set; }
        public DateTime UpdateTime { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}

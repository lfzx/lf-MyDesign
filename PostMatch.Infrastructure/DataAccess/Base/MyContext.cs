using Microsoft.EntityFrameworkCore;
using PostMatch.Core.Entities;

namespace PostMatch.Infrastructure.DataBase
{
    public class MyContext : DbContext
    {
        //构造函数
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
        }

        // 将userDto弄进来
        public DbSet<Student> Student { get; set; }
    }
}

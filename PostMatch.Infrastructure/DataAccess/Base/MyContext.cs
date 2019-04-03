using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PostMatch.Core.Entities;
using System;

namespace PostMatch.Infrastructure.DataBase
{
    public class MyContext : DbContext
    {
        //构造函数
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
        }

        // 将Student弄进来
        public DbSet<User> User { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<Administrator> Administrator { get; set; }


    }
}

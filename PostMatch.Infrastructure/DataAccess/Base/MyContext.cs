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

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>().Property(up => up).HasConversion(new BoolToZeroOneConverter<Int16>());
        //}
        

        // 将Student弄进来
        public DbSet<Student> Student { get; set; }

        public DbSet<User> User { get; set; }


    }
}

using PostMatch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Infrastructure.DataAccess.Interface
{
    //用于保存数据库操作的接口

    public interface IAlanDao
    {
        //插入数据
        bool CreateStudent(Student student);

        //读取全部记录
        IEnumerable<Student> GetStudents();

        //取某id记录
        Student GetStudentByID(int id);

        //根据id更新整条记录
        bool UpdateStudent(Student student);

        //根据id更新名称
        bool UpdateNameByID(int id, string name);

        //根据id删掉记录
        bool DeleteStudentByID(int id);
    }

}

using PostMatch.Core.Entities;
using PostMatch.Infrastructure.DataAccess.Interface;
using PostMatch.Infrastructure.DataBase;
using System.Collections.Generic;
using System.Linq;

namespace PostMatch.Infrastructure.DataAccess.Implement
{
    //用于保存数据库操作接口的实现
    public class AlanDao : IAlanDao
    {
        public MyContext Context;

        public AlanDao(MyContext context)
        {
            Context = context;
        }

        //插入数据
        public bool CreateStudent(Student student)
        {
            Context.Student.Add(student);
            return Context.SaveChanges() > 0;
        }

        //取全部记录
        public IEnumerable<Student> GetStudents()
        {
            return Context.Student.ToList();
        }

        //取某id记录
        public Student GetStudentByID(int id)
        {
            return Context.Student.SingleOrDefault(s => s.Id == id);
        }

        //根据id更新整条记录
        public bool UpdateStudent(Student student)
        {
            Context.Student.Update(student);
            return Context.SaveChanges() > 0;
        }

        //根据id更新名称
        public bool UpdateNameByID(int id, string name)
        {
            var state = false;
            var student = Context.Student.SingleOrDefault(s => s.Id == id);

            if (student != null)
            {
                student.Name = name;
                state = Context.SaveChanges() > 0;
            }

            return state;
        }

        //根据id删掉记录
        public bool DeleteStudentByID(int id)
        {
            var student = Context.Student.SingleOrDefault(s => s.Id == id);
            if(student == null)
            {
                return Context.SaveChanges() < 0;
            }   
            Context.Student.Remove(student);
            return Context.SaveChanges() > 0;
        }
    }
}

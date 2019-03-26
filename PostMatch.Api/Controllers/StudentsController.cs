using Microsoft.AspNetCore.Mvc;
using PostMatch.Core.Entities;
using PostMatch.Infrastructure.DataAccess.Interface;

namespace PostMatch.Api.Controllers
{
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        private IAlanDao AlanDao;

        public StudentController(IAlanDao alanDao)
        {
            AlanDao = alanDao;
        }

        //插入数据https://localhost:5001/api/students?name=刘强&gender=1&age=18
        [HttpPost]
        public ActionResult<string> Create(string name, byte gender, int age)
        {
            if (string.IsNullOrEmpty(name.Trim()))
            {
                return "姓名不能为空";
            }

            if (gender < 0 || gender > 2)
            {
                return "性别数据有误";
            }

            if (age <= 0)
            {
                return "年龄数据有误";
            }

            var student = new Student()
            {
                Name = name,
                Gender = gender,
                Age = age
            };

            var result = AlanDao.CreateStudent(student);

            if (result)
            {
                return "学生插入成功";
            }
            else
            {
                return "学生插入失败";
            }

        }

        //取全部记录https://localhost:5001/api/students
        [HttpGet]
        public ActionResult<string> Gets()
        {
            var names = "没有数据";
            var students = AlanDao.GetStudents();

            if (students != null)
            {
                names = "";
                foreach (var s in students)
                {
                    names += $"{s.Name} \r\n";
                }

            }

            return names;
        }

        //取某id记录https://localhost:5001/api/students/2
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var name = "没有数据";
            var student = AlanDao.GetStudentByID(id);

            if (student != null)
            {
                name = student.Name;
            }

            return name;
        }

        //根据id更新整条记录
        [HttpPut("{id}")]
        public ActionResult<string> Update(int id, string name, byte gender, int age)
        {
            if (id <= 0)
            {
                return "id 不能小于0";
            }

            if (string.IsNullOrEmpty(name.Trim()))
            {
                return "姓名不能为空";
            }

            if (gender < 0 || gender > 2)
            {
                return "性别数据有误";
            }

            if (age <= 0)
            {
                return "年龄数据有误";
            }

            var student = new Student()
            {
                Id = id,
                Name = name,
                Gender = gender,
                Age = age
            };

            var result = AlanDao.UpdateStudent(student);

            if (result)
            {
                return "学生更新成功";
            }
            else
            {
                return "学生更新失败";
            }
        }

        //根据id更新名称
        [HttpPatch("{id}")]
        public ActionResult<string> UpdateName(int id, string name)
        {
            if (id <= 0)
            {
                return "id 不能小于0";
            }

            if (string.IsNullOrEmpty(name.Trim()))
            {
                return "姓名不能为空";
            }

            var result = AlanDao.UpdateNameByID(id, name);

            if (result)
            {
                return "学生更新成功";
            }
            else
            {
                return "学生更新失败";
            }
        }

        //根据id删掉记录https://localhost:5001/api/students/6
        [HttpDelete("{id}")]
        public ActionResult<string> Delete(int id)
        {
            if (id <= 0)
            {
                return "id 不能小于0！";
            }

            var result = AlanDao.DeleteStudentByID(id);

            if (result)
            {
                return "学生删除成功";
            }
            else
            {
                return "学生删除失败";
            }
        }
    }
}

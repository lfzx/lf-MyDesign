using PostMatch.Core.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PostMatch.Api.Helpers
{
    public class ProductRepository
    {
        public ProductRepository() { }
        public DataSet GetPosts()
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT postId,companyId,postName,postDescription,city,postSalary, postWorkPlace,postJobType,academicRequirements,postExperience FROM post";
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText);
            return dataSet;
        }
        public DataSet GetResumes()
        {
            CommandType cmdType = CommandType.Text;
            string cmdText = "SELECT userId,resumeId,familyAddress,resumePostName,resumeSalary,resumeWorkPlace,resumeJobType,resumeExperience,skill,birth,workYear,profession,academic FROM resume left JOIN user on user.id = resume.userId";
            DataSet dataSet = MysqlHelper.GetDataSet(cmdType, cmdText);

            return dataSet;
        }
    }
}

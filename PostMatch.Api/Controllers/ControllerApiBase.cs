using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostMatch.Api.Controllers
{
    public abstract class ControllerApiBase : ControllerBase
    {
        public JsonResult Output(object list, int total,string msg = "ok")
        {
            return new JsonResult(new
            {
                msg,
                list,
                total
            });
        }

      
    }
}

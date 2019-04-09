using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostMatch.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostMatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AdminAppController : ControllerApiBase
    {
        [HttpGet("")]
        public JsonResult Get()
        {
            var count = 1;
            return Output(new App
            {
                project = new Project()
                {
                    name = "ng-alain"
                },
                menu = new List<Menu>()
                {
                    new Menu()
                    {
                        text = "主导航",
                        group = true,
                        children = new List<Menu>()
                        {
                            new Menu()
                            {
                                text = "主页",
                                link = "/dashboard",
                                icon = "anticon anticon-appstore-o"
                            },
                            new Menu()
                            {
                                text = "人员管理",
                                link = "/admin/userManagements",
                                icon = "anticon anticon-user",
                            },
                            new Menu()
                            {
                                text = "公司管理",
                                link = "/admin/companyManagements",
                                icon = "anticon anticon-rocket",
                            }
                        }
                    },
                    new Menu()
                    {
                        text = "业务",
                        group = true,
                        children = new List<Menu>()
                        {
                            new Menu()
                            {
                                text = "业务处理",
                                icon = "anticon anticon-skin",
                                children = new List<Menu>()
                                {
                                    new Menu()
                                    {
                                        text = "注册审核",
                                        link = "/admin/companiesUnconfirmed",
                                        icon = "anticon anticon-rocket",
                                    },
                                    new Menu()
                                    {
                                        text = "岗位审核",
                                        link = "/",
                                        icon = "anticon anticon-rocket",
                                    }
                                }
                            }
                        }
                    }
                }
            },count);
        }
    }
}

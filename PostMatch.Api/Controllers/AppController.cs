using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using PostMatch.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace PostMatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AppController : ControllerApiBase
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
                                text = "公司推荐",
                                link = "/user/jobRecommendations",
                                icon = "anticon anticon-bulb"
                            },
                            new Menu()
                            {
                                text = "个人简历",
                                link = "/user/resumes",
                                icon = "anticon anticon-profile",
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
                                        text = "简历投递情况",
                                        link = "/user/deliveries",
                                        icon = "anticon anticon-check-circle"
                                    },
                                      new Menu()
                                    {
                                        text = "面试邀请情况",
                                        link = "/user/interview",
                                        icon = "anticon anticon-link"
                                    }
                                }
                            }
                }
            },count);
        }
    }
}

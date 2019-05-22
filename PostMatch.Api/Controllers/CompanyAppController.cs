using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostMatch.Api.Models;
using System.Collections.Generic;

namespace PostMatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CompanyAppController : ControllerApiBase
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
                                text = "招聘信息",
                                link = "/company/posts",
                                icon = "anticon anticon-profile",
                            },
                             new Menu()
                            {
                                text = "简历推荐",
                                link = "/company/employeeRecommendations",
                                icon = "anticon anticon-bulb"
                            },
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
                                        text = "投递情况",
                                        link = "/company/deliveries",
                                        icon = "anticon anticon-check-circle"
                                    },
                                      new Menu()
                                    {
                                        text = "面试邀请",
                                        link = "/company/inerview",
                                        icon = "anticon anticon-link"
                                    }
                                }
                    }
                }
            },count);
        }
    }
}

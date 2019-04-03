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
                                text = "优才推荐",
                                link = "/company/employeeRecommendations",
                                icon = "anticon anticon-appstore-o"
                            },
                            new Menu()
                            {
                                text = "招聘信息",
                                link = "/company/posts",
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
                                        text = "投递情况",
                                        link = "/company/deliveries"
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}

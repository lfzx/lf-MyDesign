using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostMatch.Api.Models;
using PostMatch.Infrastructure.DataBase;
using System.Linq;

namespace PostMatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController: Controller
    {
        private readonly MyContext db;

        public TestController(MyContext db){
            this.db = db;
        }

        public IActionResult Index()
        {
            var Info = db.User.OrderBy(t => t.Id)
                .Select(t => new UserModel
                {
                    Id= t.Id,
                    Name = t.Name
                }).ToList();
            return Ok(Info);
        }
    }
}

using Microsoft.AspNetCore.Mvc;


namespace PostMatch.Api.Controllers
{
    [Route("api/posts")]
    public class PostController:Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            //UserContext context = HttpContext.RequestServices.GetService(typeof(UserContext)) as UserContext;
            //var data = context.GetAllUser();
            return Ok("hello World");
        }
    }
}

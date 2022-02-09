using Microsoft.AspNetCore.Mvc;
using PasswordManager.Database;
using PasswordManager.Database.Entities;
using PasswordManager.Entities;

namespace PasswordManager.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
       private readonly ILogger<WeatherForecastController> _logger;

        public UserController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        [Route("/getUser")]
        public async Task<IActionResult> Get()
        {
            using var db = new PasswordManagerContext();
            var users =  db.Users.ToList();
            return Ok(users);
        }

        [HttpPost]
        [Route("/addUser")]
        public IActionResult AddUser([FromBody] User user)
        {
            using var db = new PasswordManagerContext();
            db.Users.Add(new User { Name = user.Name });
            db.SaveChanges();

            return Ok("Sucessful");
        }
    }
}

using auth.Data;
using auth.Models;
using auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace auth.Controller
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly TokenService _tokenService;
        public AuthController(IUserRepository repository, TokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] CreateRegister body)
        {
            var user = new User
            {
                Name = body.Name,
                Email = body.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(body.Password),
            };
            _repository.Create(user);
            return Ok(user);
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginAccount body)
        {
            var user = _repository.GetByEmail(body.Email);
            if (user == null) return BadRequest("Email and password are not valid");
            if (!BCrypt.Net.BCrypt.Verify(body.Password, user.Password))
            {
                return BadRequest("Email and password are not valid");
            }
            var jwt = _tokenService.Generate(user);
            // Response.Cookies.Append("jwt", jwt, new CookieOptions
            // {
            //     HttpOnly = true
            // });
            return Ok(jwt);
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok("Logout success");
        }
        [HttpGet("user")]
        public IActionResult GetUser()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];
                var token = _tokenService.Verify(jwt);
                int UserId = int.Parse(token.Issuer);
                var user = _repository.GetById(UserId);
                return Ok(user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
        }
        public class CreateRegister
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class LoginAccount
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
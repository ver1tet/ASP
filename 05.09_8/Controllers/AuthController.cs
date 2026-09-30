using Microsoft.AspNetCore.Mvc;

namespace AspHomework.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Username == "admin" && request.Password == "admin123")
        {
            return Ok(new
            {
                success = true,
                message = "Авторизація успішна",
                token = "jwt.token.mock.claims",
                user = new { id = 1, role = "Administrator" }
            });
        }

        return Unauthorized(new
        {
            success = false,
            message = "Невірний логін або пароль"
        });
    }

    [HttpGet("protected")]
    public IActionResult ProtectedEndpoint([FromHeader(Name = "Authorization")] string authHeader)
    {
        if (authHeader == "Bearer jwt.token.mock.claims")
        {
            return Ok(new { message = "Доступ дозволено. Ваші Claims: [Role: Administrator]" });
        }
        
        return Unauthorized(new { message = "Доступ заборонено. Відсутній або невірний токен." });
    }
}

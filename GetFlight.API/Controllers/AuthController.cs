using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GetFlight.API.Controllers
{
    public class AuthController
    {
        /// <summary>
        /// Контроллер для аутентификации пользователей
        /// </summary>
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
            private readonly IConfiguration _configuration;
            private readonly ILogger<AuthController> _logger;

            public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
            {
                _configuration = configuration;
                _logger = logger;
            }

            /// <summary>
            /// Аутентификация пользователя и получение JWT-токена
            /// </summary>
            /// <param name="request">Учетные данные пользователя</param>
            /// <returns>JWT-токен для доступа к API</returns>
            /// <response code="200">Успешная аутентификация</response>
            /// <response code="401">Неверные учетные данные</response>
            [HttpPost("login")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
            {
                // Для тестового задания используем упрощенную проверку
                // В реальном приложении здесь была бы проверка в базе данных
                if (IsValidUser(request.Username, request.Password))
                {
                    _logger.LogInformation("User {Username} successfully authenticated", request.Username);
                    var token = GenerateJwtToken(request.Username);
                    return Ok(new LoginResponse { Token = token });
                }

                _logger.LogWarning("Failed login attempt for user {Username}", request.Username);
                return Unauthorized();
            }

            private bool IsValidUser(string username, string password)
            {
                // Для тестового задания проверяем фиксированные учетные данные
                return (username == "user" && password == "password") ||
                       (username == "admin" && password == "admin123");
            }

            private string GenerateJwtToken(string username)
            {
                var securityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var credentials = new SigningCredentials(
                    securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User")
            };

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(
                        Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "60")),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
    }
}

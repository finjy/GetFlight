using System.ComponentModel.DataAnnotations;

namespace GetFlight.API.Models
{
    /// <summary>
    /// Модель запроса для аутентификации
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}

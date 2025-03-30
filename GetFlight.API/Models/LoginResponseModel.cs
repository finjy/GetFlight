namespace GetFlight.API.Models
{
    /// <summary>
    /// Модель ответа с токеном аутентификации
    /// </summary>
    public class LoginResponseModel
    {
        /// <summary>
        /// JWT-токен для доступа к API
        /// </summary>
        public string Token { get; set; }
    }
}

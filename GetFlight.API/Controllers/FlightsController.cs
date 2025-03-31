using GetFlight.Application.DTOs;
using GetFlight.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetFlight.API.Controllers
{
    /// <summary>
    /// Контроллер для работы с перелетами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly ILogger<FlightsController> _logger;

        public FlightsController(IFlightService flightService, ILogger<FlightsController> logger)
        {
            _flightService = flightService;
            _logger = logger;
        }

        /// <summary>
        /// Поиск доступных перелетов
        /// </summary>
        /// <param name="request">Параметры поиска</param>
        /// <returns>Список доступных перелетов</returns>
        /// <response code="200">Список найденных перелетов</response>
        /// <response code="400">Некорректные параметры запроса</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<FlightDto>>> SearchFlights([FromQuery] SearchFlightRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Searching flights from {Origin} to {Destination} on {DepartureDate}",
                request.Origin, request.Destination, request.DepartureDate.ToString("yyyy-MM-dd"));

            var flights = await _flightService.SearchFlightsAsync(request, cancellationToken);
            return Ok(flights);
        }

        /// <summary>
        /// Бронирование выбранного рейса
        /// </summary>
        /// <param name="request">Информация для бронирования</param>
        /// <returns>Результат бронирования</returns>
        /// <response code="200">Бронирование выполнено успешно</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Пользователь не аутентифицирован</response>
        [HttpPost("book")]
        [Authorize] // Требуется аутентификация для бронирования
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BookingResultDto>> BookFlight([FromBody] BookingRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Booking flight {FlightId} from provider {Provider}",
                request.FlightId, request.Provider);

            var result = await _flightService.BookFlightAsync(request, cancellationToken);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                // возвращаем статус 200, но с информацией о неудаче, так как это
                // бизнес-логика, а не ошибка API
                return Ok(result);
            }
        }
    }
}

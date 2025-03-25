# GetFlight API

## Project Overview
GetFlight is an API for aggregating available flight data from different sources. The application provides a unified interface for searching, filtering, and booking flights.

## Key Features
- Flight data aggregation from various sources
- Filtering and sorting search results by different parameters
- Booking selected flights
- Caching frequently requested routes
- Detailed request logging
- Handling long response times from data sources

## Tech Stack
- ASP.NET Core 8.0
- RESTful API with Swagger/OpenAPI documentation
- JWT authentication
- LazyCache for caching
- Serilog for logging
- xUnit, Moq, and FluentAssertions for testing

## Architecture
The project is implemented using a multi-layer architecture:
- GetFlight.API - REST API controllers and middleware
- GetFlight.Application - business logic and services
- GetFlight.Domain - domain models and interfaces
- GetFlight.Infrastructure - implementation of external providers and infrastructure services

## Running the Project
The application can be run locally or in a Docker container.

### Local Launch
```bash
cd GetFlight
dotnet restore
dotnet build
dotnet run --project src/GetFlight.API
```

### Docker Launch
```bash
docker build -t getflight .
docker run -p 8080:80 getflight
```

## API
After launch, the API documentation is available at: http://localhost:5000/swagger

### Main Endpoints
- `GET /api/flights` - search for available flights with filtering and sorting
- `POST /api/flights/book` - book a selected flight
- `POST /api/auth/login` - get a JWT token for API access

## Testing
The project includes unit tests for key system components:
```bash
dotnet test
```

## Notes
This project is a test assignment demonstrating an approach to creating an aggregating API for flights. For real use, additional configuration and integration with actual data providers would be required.

---

# GetFlight API

## Обзор проекта
GetFlight - это API для агрегации данных о доступных авиаперелетах из разных источников. Приложение предоставляет унифицированный интерфейс для поиска, фильтрации и бронирования рейсов.

## Основные возможности
- Агрегация данных о перелетах из различных источников
- Фильтрация и сортировка результатов поиска по различным параметрам
- Бронирование выбранных рейсов
- Кэширование часто запрашиваемых маршрутов
- Детальное логирование запросов
- Обработка долгих ответов от источников данных

## Технический стек
- ASP.NET Core 6.0+
- RESTful API с документацией Swagger/OpenAPI
- JWT-аутентификация
- LazyCache для кэширования
- Serilog для логирования
- xUnit, Moq и FluentAssertions для тестирования

## Архитектура
Проект реализован с использованием многослойной архитектуры:
- GetFlight.API - REST API контроллеры и middleware
- GetFlight.Application - бизнес-логика и сервисы
- GetFlight.Domain - доменные модели и интерфейсы
- GetFlight.Infrastructure - реализация внешних провайдеров и инфраструктурных сервисов

## Запуск проекта
Приложение можно запустить локально или в Docker-контейнере.

### Локальный запуск
```bash
cd GetFlight
dotnet restore
dotnet build
dotnet run --project src/GetFlight.API
```

### Запуск в Docker
```bash
docker build -t getflight .
docker run -p 8080:80 getflight
```

## API
После запуска документация API доступна по адресу: http://localhost:5000/swagger

### Основные эндпоинты
- `GET /api/flights` - поиск доступных перелетов с фильтрацией и сортировкой
- `POST /api/flights/book` - бронирование выбранного рейса
- `POST /api/auth/login` - получение JWT-токена для доступа к API

## Тестирование
Проект включает модульные тесты для ключевых компонентов системы:
```bash
dotnet test
```

## Примечания
Этот проект является тестовым заданием, демонстрирующим подход к созданию агрегирующего API для рейсов. Для реального использования потребуется дополнительная настройка и интеграция с реальными поставщиками данных.

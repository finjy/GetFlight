# GetFlight API (English version below)

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

## Тестирование API с помощью Swagger

### Шаг 1: Доступ к Swagger UI
1. Запустите приложение используя один из методов, описанных выше
2. Откройте браузер и перейдите по адресу http://localhost:5000/swagger или https://localhost:5001/swagger
3. Вы увидите интерфейс Swagger со всеми доступными эндпоинтами

### Шаг 2: Аутентификация
1. Разверните эндпоинт `/api/Auth/login`
2. Нажмите кнопку "Try it out"
3. Введите тестовые учетные данные в теле запроса:
   ```json
   {
     "username": "user",
     "password": "password"
   }
   ```
4. Нажмите "Execute"
5. Вы должны получить ответ со статусом 200 и JWT-токеном
6. Скопируйте значение токена (без кавычек)
7. Нажмите кнопку "Authorize" вверху страницы
8. В поле значения введите `Bearer ` и вставьте ваш токен (например, `Bearer eyJhbGciOiJIUzI1NiI...`)
9. Нажмите "Authorize", а затем "Close"

### Шаг 3: Поиск рейсов
1. Разверните эндпоинт `GET /api/Flights`
2. Нажмите "Try it out"
3. Заполните обязательные параметры:
   - `origin`: "MOW" (Москва)
   - `destination`: "LED" (Санкт-Петербург)
   - `departureDate`: Введите будущую дату в формате YYYY-MM-DD (например, "2025-04-15")
   - `passengers`: 2
4. При желании можно указать дополнительные параметры фильтрации:
   - `maxPrice`: Максимальная цена (например, 200)
   - `airline`: Название авиакомпании для фильтрации (например, "First Airlines" или "Second Air")
   - `sortBy`: Поле для сортировки (например, "price", "duration", "departure")
   - `sortOrder`: "asc" или "desc"
5. Нажмите "Execute"
6. Вы должны получить список доступных рейсов, соответствующих вашим критериям

### Шаг 4: Бронирование рейса
1. Сначала выполните поиск рейсов, как описано в Шаге 3
2. Из результатов поиска запомните `id` и `provider` рейса, который вы хотите забронировать
3. Разверните эндпоинт `POST /api/Flights/book`
4. Нажмите "Try it out"
5. Введите данные для бронирования в теле запроса:
   ```json
   {
     "flightId": "guid-из-результатов-поиска",
     "provider": "имя-провайдера-из-результатов",
     "passengers": [
       {
         "firstName": "Иван",
         "lastName": "Иванов",
         "dateOfBirth": "1990-01-01",
         "passportNumber": "AB123456"
       }
     ]
   }
   ```
6. Нажмите "Execute"
7. Вы должны получить подтверждение бронирования с номером бронирования

### Важные примечания
- Эндпоинт бронирования требует аутентификации, убедитесь, что вы выполнили Шаг 2
- Если вы видите ошибку 401 Unauthorized, возможно, срок действия вашего токена истек - повторите Шаг 2
- Оба провайдера полетов симулируются, поэтому все данные генерируются случайным образом
- Иногда бронирование может не удаться из-за симулированной недоступности (80-90% успешных бронирований)

## Тестирование
Проект включает модульные тесты для ключевых компонентов системы:
```bash
dotnet test
```

## Примечания
Этот проект является тестовым заданием, демонстрирующим подход к созданию агрегирующего API для рейсов. Для реального использования потребуется дополнительная настройка и интеграция с реальными поставщиками данных.

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
- ASP.NET Core 6.0+
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

## Testing the API with Swagger

### Step 1: Accessing Swagger UI
1. Launch the application using one of the methods described above
2. Open your browser and navigate to http://localhost:5000/swagger or https://localhost:5001/swagger
3. You'll see the Swagger UI with all available endpoints

### Step 2: Authentication
1. Expand the `/api/Auth/login` endpoint
2. Click the "Try it out" button
3. Enter test credentials in the request body:
   ```json
   {
     "username": "user",
     "password": "password"
   }
   ```
4. Click "Execute"
5. You should receive a response with status 200 and a JWT token
6. Copy the token value (without quotes)
7. Click the "Authorize" button at the top of the page
8. In the value field, enter `Bearer ` followed by your token (e.g., `Bearer eyJhbGciOiJIUzI1NiI...`)
9. Click "Authorize" and then "Close"

### Step 3: Searching for Flights
1. Expand the `GET /api/Flights` endpoint
2. Click "Try it out"
3. Fill in the required parameters:
   - `origin`: "MOW" (Moscow)
   - `destination`: "LED" (St. Petersburg)
   - `departureDate`: Enter a future date in YYYY-MM-DD format (e.g., "2025-04-15")
   - `passengers`: 2
4. Optionally, you can specify additional filter parameters:
   - `maxPrice`: Maximum price (e.g., 200)
   - `airline`: Airline name for filtering (e.g., "First Airlines" or "Second Air")
   - `sortBy`: Field to sort by (e.g., "price", "duration", "departure")
   - `sortOrder`: "asc" or "desc"
5. Click "Execute"
6. You should receive a list of available flights matching your criteria

### Step 4: Booking a Flight
1. First, search for flights as described in Step 3
2. From the search results, note the `id` and `provider` of the flight you want to book
3. Expand the `POST /api/Flights/book` endpoint
4. Click "Try it out"
5. Enter the booking details in the request body:
   ```json
   {
     "flightId": "guid-from-search-results",
     "provider": "provider-name-from-results",
     "passengers": [
       {
         "firstName": "John",
         "lastName": "Doe",
         "dateOfBirth": "1990-01-01",
         "passportNumber": "AB123456"
       }
     ]
   }
   ```
6. Click "Execute"
7. You should receive a booking confirmation with a booking reference

### Important Notes
- The booking endpoint requires authentication, make sure you've completed Step 2
- If you see a 401 Unauthorized error, your token may have expired - repeat Step 2
- Both flight providers are simulated, so all data is generated randomly
- Sometimes booking may fail due to simulated unavailability (80-90% success rate)

## Testing
The project includes unit tests for key system components:
```bash
dotnet test
```

## Notes
This project is a test assignment demonstrating an approach to creating an aggregating API for flights. For real use, additional configuration and integration with actual data providers would be required.

---


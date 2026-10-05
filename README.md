# TaskManager

Список задач на Avalonia UI и PostgreSQL (EF Core 8). Один проект UI `TaskManager.Desktop`, тесты в `TaskManagerTests`.

![Интерфейс](UI.png)

## Запуск

Нужны .NET SDK 8 и Docker.

```bash
docker compose up -d
dotnet run --project TaskManager.Desktop
```

Postgres: порт **5433**, база `tasks`, пользователь и пароль `taskmanager`. Миграция `InitialCreate` применяется при старте (`Database.Migrate()`). Если база недоступна, окно всё равно открывается.

## Архитектура

MVVM внутри одного десктопного проекта.

- **View** — `MainWindow.axaml`.
- **ViewModel** — `MainViewModel` (список, ввод, ошибки, команды), `TaskRowViewModel` (строка таблицы).
- **Сервис** — `TaskService`: валидация FluentValidation, AutoMapper.
- **Репозиторий** — `TaskRepository` над `DbSet`, мягкое удаление, транзакция на чтение+запись.
- **Данные** — `TaskDbContext` (scoped), query filter `!IsDeleted`.

Ошибки ловит `ExceptionHandlingMiddleware`.

## Тесты

```bash
dotnet test --project TaskManagerTests
```

Нужен запущенный Docker: репозиторий гоняется на эфемерном Postgres (Testcontainers).

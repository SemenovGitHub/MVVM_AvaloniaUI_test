# TaskManager

Список задач: кроссплатформенное десктопное приложение на Avalonia UI с хранением данных в PostgreSQL через EF Core.

Один проект `TaskManager.Desktop` — структура повторяет рекомендованную в задании.

## Сборка и запуск

Нужны .NET SDK 8 и Docker.

```bash
docker compose up -d
dotnet run --project TaskManager.Desktop
```

PostgreSQL поднимается на порту **5433**, база `tasks`, пользователь и пароль `taskmanager`.

## Интерфейс

Таблица задач, поле ввода сверху, сообщение об ошибке под ним. Окно масштабируется: колонка с названием растягивается, остальные держат ширину, минимальный размер ограничен.

Клавиатура: `Enter` в поле ввода добавляет задачу, `Delete` удаляет выбранную строку, `F5` обновляет список, `Tab` переводит фокус.

## Миграции

При старте приложение само вызывает `Database.Migrate()`. Если база недоступна, окно всё равно открывается: ошибка уходит в лог, а операции с данными показывают сообщение в интерфейсе.

Ручное применение:

```bash
dotnet tool restore
dotnet ef database update --project TaskManager.Desktop
```

Новая миграция:

```bash
dotnet ef migrations add <Name> --project TaskManager.Desktop --output-dir Data/Migrations
```

Первая миграция уже создана: `InitialCreate`.

## Структура

```
TaskManager.Desktop/
├── Models/        бизнес-модель TaskItem и ограничение IBusinessModel
├── Services/      сервисы и репозитории: интерфейсы и реализации
├── Data/          EF Core: TaskDbContext, Entities, Configurations, Migrations
├── Mapping/       профиль AutoMapper
├── Validation/    правила FluentValidation
├── Errors/        NotFoundException и тексты ошибок для интерфейса
├── Middleware/    перехват исключений вокруг операций
├── ViewModels/    MainViewModel, TaskRowViewModel
├── Views/         MainWindow.axaml и минимальный code-behind
├── DI/            регистрация сервисов и сборка провайдера
├── App.axaml
└── Program.cs
```

## Архитектура

Слои разведены по папкам внутри одного проекта.

- **Domain** — `Models`, `Services`, `Validation`. Общий CRUD вынесен в `ServiceBase<TModel, TEntity, TRepository>` / `IServiceBase<TModel>`: список, запись по идентификатору, создание, мягкое удаление. Доступ к полям внутри дженерика дают ограничения `IBusinessModel` для модели и `IEntity` для сущности. `TaskService` / `ITaskService` наследуют базу и добавляют `SetCompletionAsync`, потому что `IsCompleted` есть только у задачи. Каждая операция сервиса — ровно один вызов репозитория; сервис отвечает за валидацию, маппинг и логирование.
- **Infrastructure** — `Data` и `Mapping`. `TaskDbContext` зарегистрирован scoped, схема описана через `IEntityTypeConfiguration`. Generic `RepositoryBase<TEntity>` работает поверх `DbSet<TEntity>`, типизированная пара `ITaskRepository` / `TaskRepository` добавляет смену признака выполнения. Репозиторий инкапсулирует работу с контекстом целиком: поиск, пометку удаления, транзакции и вызов `SaveChangesAsync`. Отсутствие записи он сам сообщает исключением `NotFoundException`; текст задаёт `NotFoundMessage`, который наследник переопределяет. AutoMapper 15.1.1 при локальном запуске пишет предупреждение о лицензии: для разработки и проверки это штатно.
- **Presentation** — `Views`, `ViewModels`, `Middleware`. `MainWindow.axaml` содержит только разметку и привязки, code-behind сводится к `InitializeComponent`. `MainViewModel` держит коллекцию строк, команды и текст ошибки; `TaskRowViewModel` оборачивает одну задачу и отдаёт команды чекбокса и удаления. `INotifyPropertyChanged` и `ICommand` приходят из `CommunityToolkit.Mvvm`.

### Обработка ошибок

Сервисы, репозитории и ViewModel ошибки не обрабатывают — только бросают. Перехватывает их `ExceptionHandlingMiddleware`: он оборачивает операцию, логирует её (`LogWarning` для валидации и «не найдено», `LogError` для остального) и возвращает текст для интерфейса либо `null` при успехе. `MainViewModel` кладёт этот текст в `ErrorMessage`, и он показывается строкой под полем ввода. Тексты формирует `ErrorText.Resolve`: сообщение валидатора, сообщение `NotFoundException`, «ошибка базы данных» (ищется `DbUpdateException` или `NpgsqlException` по всей цепочке `InnerException`) или общая «непредвиденная ошибка». Технические детали остаются в логах.

Исключения, возникшие вне этого пути, подхватывают `AppDomain.UnhandledException` и `TaskScheduler.UnobservedTaskException` в `Program`: они логируются, процесс не падает.

### DbContext и время жизни

`DbContext` зарегистрирован как scoped, а `MainViewModel` живёт всё время работы окна. Поэтому ViewModel не держит сервис в поле: каждая операция берёт свой scope через `IServiceScopeFactory` и получает свежий `ITaskService`. Так контекст не превращается в долгоживущий кэш отслеживаемых сущностей.

### Транзакции

Операции, где есть и чтение, и запись (мягкое удаление, смена признака выполнения), выполняются в явной транзакции: `ExecuteInTransactionAsync` в `RepositoryBase` открывает транзакцию, выполняет действие, сохраняет и коммитит. Исключение или отмена токена приводят к откату, потому что транзакция освобождается без коммита. Создание транзакции не требует: это один `SaveChangesAsync`. Уровень изоляции по умолчанию, токена конкуренции у сущности нет, поэтому одновременное изменение одной задачи из двух мест не вызовет конфликт — побеждает последняя запись.

### Мягкое удаление

У строки выставляются `IsDeleted` и `DeletedAt`. Скрывает такие строки общее правило: `OnModelCreating` проходит по всем сущностям модели и каждой, реализующей `IEntity`, ставит query filter `entity => !entity.IsDeleted`. Отдельной настройки в конфигурации сущности не требуется, при необходимости фильтр снимается через `IgnoreQueryFilters()`.

## Что сделано и что нет

Сделано:

- модель задачи, таблица задач, добавление по `Enter`, чекбокс выполнения, мягкое удаление;
- MVVM: разметка и привязки в XAML, логика и команды в ViewModel, `CommunityToolkit.Mvvm` для `INotifyPropertyChanged` и `ICommand`;
- адаптивное окно, сообщение об ошибке в интерфейсе, клавиатурная навигация;
- EF Core 8 и провайдер PostgreSQL, миграция `InitialCreate`, применение при старте;
- `DbContext` со scoped lifetime, репозиторий над `DbSet`, scope на операцию;
- асинхронные операции с базой, DI через `Microsoft.Extensions.DependencyInjection`, `ILogger<T>`;
- FluentValidation от бизнес-модели: `TaskItemValidator` внедряется в сервис как `IValidator<TaskItem>`;
- AutoMapper как единственное место преобразования моделей;
- middleware для перехвата исключений, включая ошибки базы: приложение не падает.

Не сделано:

- unit-тесты;
- отдельного окна `AddEditTaskWindow` нет: задача добавляется строкой ввода над таблицей, как требует пункт про `Enter`;
- файл `tasks.db`. В задании одновременно указаны локальный файл и провайдер PostgreSQL. Реализован PostgreSQL: это серверная база, её нельзя положить в один файл приложения как SQLite;
- скриншотов интерфейса в README нет.

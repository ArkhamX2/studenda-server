using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Studenda.Server.Configuration.Repository;
using Studenda.Server.Data;
using Studenda.Server.Data.Initialization;
using Studenda.Server.Middleware;
using Studenda.Server.Middleware.Security;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Service;
using Studenda.Server.Service.Journal;
using Studenda.Server.Service.Schedule;
using Studenda.Server.Service.Security;
using ConfigurationManager = Studenda.Server.Configuration.ConfigurationManager;

/// <summary>
///     Класс для запуска приложения.
/// </summary>
internal class Program
{
#if DEBUG
    private const bool IsDebugMode = true;
#else
    private const bool IsDebugMode = false;
#endif

    /// <summary>
    ///     Точка входа в приложение.
    /// </summary>
    /// <param name="args">Аргументы из командной строки.</param>
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = new ConfigurationManager(builder.Configuration);

        RegisterCoreServices(builder.Services);
        RegisterDataSources(builder.Services, configuration.DataConfiguration);
        RegisterIdentityServices(builder.Services, configuration.IdentityConfiguration);
        RegisterAuthorizationServices(builder.Services);
        RegisterAuthenticationServices(builder.Services, configuration.TokenConfiguration);
        RegisterCorsServices(builder.Services);

        var application = builder.Build();

        RunApplication(application);
    }

    /// <summary>
    ///    Запустить приложение.
    /// </summary>
    /// <param name="application">Приложение.</param>
    private static void RunApplication(WebApplication application)
    {
        application.UseMiddleware<ExceptionHandler>(IsDebugMode);
        application.UseAuthentication();
        application.UseAuthorization();
        application.MapControllers();
        application.UseCors();

        InitializeDataSources(application);
        InitializeSwagger(application);

        application.Run();
    }

    /// <summary>
    ///     Зарегистрировать основные сервисы и контроллеры.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddScoped<GroupService>();

        services.AddScoped<TokenService>();
        services.AddScoped<SecurityService>();

        services.AddScoped<DataEntityService>();
        services.AddScoped<AccountService>();
        services.AddScoped<RoleService>();
        services.AddScoped<SubjectService>();
        services.AddScoped<WeekTypeService>();

        services.AddScoped<TaskService>();
        services.AddScoped<AbsenceService>();
        services.AddScoped<SessionService>();

        services.AddTransient<ConfigurationManager>();
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    /// <summary>
    ///     Зарегистрировать источники данных.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурации данных.</param>
    private static void RegisterDataSources(IServiceCollection services, DataConfiguration configuration)
    {
        var dataConfiguration = configuration.GetDefaultContextConfiguration(IsDebugMode);

        services.AddScoped(provider => new DataContext(dataConfiguration));
        services.AddScoped<DataInitializationScript>();
    }

    /// <summary>
    ///     Зарегистрировать сервисы идентификации.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурации модуля идентификации.</param>
    private static void RegisterIdentityServices(IServiceCollection services, IdentityConfiguration configuration)
    {
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<DataContext>()
            .AddUserManager<UserManager<IdentityUser>>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddSignInManager<SignInManager<IdentityUser>>();

        services.Configure<IdentityOptions>(options => configuration.GetOptions());
    }

    /// <summary>
    ///     Зарегистрировать сервисы авторизации.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    private static void RegisterAuthorizationServices(IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(
                DefaultAuthorizationRequirement.PolicyCode,
                policy => policy.Requirements.Add(new DefaultAuthorizationRequirement()))
            .AddPolicy(
                LeaderAuthorizationRequirement.PolicyCode,
                policy => policy.Requirements.Add(new LeaderAuthorizationRequirement()))
            .AddPolicy(
                TeacherAuthorizationRequirement.PolicyCode,
                policy => policy.Requirements.Add(new TeacherAuthorizationRequirement()))
            .AddPolicy(
                AdminAuthorizationRequirement.PolicyCode,
                policy => policy.Requirements.Add(new AdminAuthorizationRequirement()));

        services.AddSingleton<IAuthorizationHandler, PolicyAuthorizationHandler<DefaultAuthorizationRequirement>>();
        services.AddSingleton<IAuthorizationHandler, PolicyAuthorizationHandler<LeaderAuthorizationRequirement>>();
        services.AddSingleton<IAuthorizationHandler, PolicyAuthorizationHandler<TeacherAuthorizationRequirement>>();
        services.AddSingleton<IAuthorizationHandler, PolicyAuthorizationHandler<AdminAuthorizationRequirement>>();
    }

    /// <summary>
    ///     Зарегистрировать сервисы аутентификации.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурация токенов.</param>
    private static void RegisterAuthenticationServices(IServiceCollection services, TokenConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = configuration.GetValidationParameters();
        });
    }

    /// <summary>
    ///     Зарегистрировать сервисы межсайтовой аутентификации.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    private static void RegisterCorsServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.SetIsOriginAllowed(origin => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    /// <summary>
    ///     Инициализировать сессии данных.
    /// </summary>
    /// <param name="application">Приложение.</param>
    private static async void InitializeDataSources(WebApplication application)
    {
        using var scope = application.Services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<DataInitializationScript>().Run();
    }

    /// <summary>
    ///     Инициализировать Swagger.
    /// </summary>
    /// <param name="application">Приложение.</param>
    private static void InitializeSwagger(WebApplication application)
    {
        if (application.Environment.IsDevelopment())
        {
            application.UseSwagger();
            application.UseSwaggerUI();
        }
    }
}
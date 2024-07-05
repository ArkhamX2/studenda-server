using Studenda.Server.Service.Security;

namespace Studenda.Server.Data.Initialization;

/// <summary>
///     Скрипт инициализации контекста данных.
/// </summary>
/// <param name="dataContext">Контекст данных.</param>
/// <param name="securityService">Сервис работы с безопасностью.</param>
class DataInitializationScript(
    DataContext dataContext,
    SecurityService securityService
) : IInitializationScript
{
    private DataContext DataContext { get; } = dataContext;
    private SecurityService SecurityService { get; } = securityService;

    /// <summary>
    ///     Запустить инициализацию контекста данных.
    /// </summary>
    /// <returns>Операция.</returns>
    /// <exception cref="Exception">При ошибке инициализации.</exception>
    public async Task Run()
    {
        if (!await DataContext.TryInitializeAsync())
        {
            throw new Exception("Data initialization failed!");
        }

        await SecurityService.CreateDefaultUser();
    }
}
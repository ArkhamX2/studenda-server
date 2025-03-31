using System.Net;

namespace Studenda.Server.Middleware;

/// <summary>
///    Обработчик исключений.
/// </summary>
/// <param name="requestDelegate">Делегат запроса.</param>
/// <param name="isDebugMode">Статус режима отладки.</param>
public class ExceptionHandler(RequestDelegate requestDelegate, bool isDebugMode)
{
    private RequestDelegate RequestDelegate { get; } = requestDelegate;
    private bool IsDebugMode { get; } = isDebugMode;

    /// <summary>
    ///     Вызвать обработку асинхронно.
    /// </summary>
    /// <param name="context">Контекст запроса.</param>
    /// <returns>Операция.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await RequestDelegate.Invoke(context);
        }
        catch (Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                ErrorType = exception.GetType().ToString(),
                ErrorMessage = exception.Message,
                InnerException = IsDebugMode ? exception.InnerException?.ToString() : null
            });
        }
    }
}
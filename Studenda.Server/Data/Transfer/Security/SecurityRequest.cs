namespace Studenda.Server.Data.Transfer.Security;

/// <summary>
///     Тело запроса модуля безопасности.
/// </summary>
public class SecurityRequest
{
    /// <summary>
    ///     Пароль.
    /// </summary>
    public required string Password { get; init; }
}
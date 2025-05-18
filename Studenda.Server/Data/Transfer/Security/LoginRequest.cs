namespace Studenda.Server.Data.Transfer.Security;

/// <summary>
///     Тело запроса авторизации.
/// </summary>
public class LoginRequest : SecurityRequest
{
    /// <summary>
    ///     Почта.
    /// </summary>
    public required string Email { get; init; }
}
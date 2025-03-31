using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Security;
using Studenda.Server.Service.Security;

namespace Studenda.Server.Controller.Security;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="Account" />.
/// </summary>
/// <param name="accountService">Сервис моделей.</param>
[Route(UpstreamConfiguration.SecurityAccount)]
[ApiController]
public class AccountController(AccountService accountService) : ControllerBase
{
    private AccountService AccountService { get; } = accountService;

    /// <summary>
    ///     Получить список аккаунтов.
    ///     Если идентификаторы не указаны, возвращается список со всеми аккаунтами.
    ///     Иначе возвращается список с указанными аккаунтами, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком аккаунтов.</returns>
    [HttpGet]
    public async Task<ActionResult<List<Account>>> Get([FromQuery] List<int> ids)
    {
        return await AccountService.Get(AccountService.DataContext.Accounts, ids);
    }

    /// <summary>
    ///     Получить список аккаунтов по идентификаторам групп.
    /// </summary>
    /// <param name="groupIds">Идентификаторы групп.</param>
    /// <returns>Результат операции со списком аккаунтов.</returns>
    [HttpGet("group")]
    public async Task<ActionResult<List<Account>>> GetByGroup([FromQuery] List<int> groupIds)
    {
        return await AccountService.GetByGroup(groupIds);
    }

    /// <summary>
    ///     Получить список аккаунтов по идентификаторам ролей.
    /// </summary>
    /// <param name="roleIds">Идентификаторы ролей.</param>
    /// <returns>Результат операции со списком аккаунтов.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpGet("role")]
    public async Task<ActionResult<List<Account>>> GetByRole([FromQuery] List<int> roleIds)
    {
        return await AccountService.GetByRole(roleIds);
    }

    /// <summary>
    ///     Сохранить аккаунты.
    /// </summary>
    /// <param name="entities">Список аккаунтов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<Account> entities)
    {
        var status = await AccountService.Set(entities);

        if (!status)
        {
            return BadRequest("No accounts were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить аккаунты.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] List<int> ids)
    {
        var status = await AccountService.Remove(ids);

        if (!status)
        {
            return BadRequest("No accounts were deleted!");
        }

        return Ok();
    }
}
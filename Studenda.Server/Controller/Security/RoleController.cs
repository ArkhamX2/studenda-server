using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Security;
using Studenda.Server.Service.Security;

namespace Studenda.Server.Controller.Security;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="Role" />.
/// </summary>
/// <param name="roleService">Сервис моделей.</param>
[Route(UpstreamConfiguration.SecurityRole)]
[ApiController]
public class RoleController(RoleService roleService) : ControllerBase
{
    private RoleService RoleService { get; } = roleService;

    /// <summary>
    ///     Получить список ролей.
    ///     Если идентификаторы не указаны, возвращается список со всеми ролями.
    ///     Иначе возвращается список с указанными ролями, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком ролей.</returns>
    [HttpGet]
    public async Task<ActionResult<List<Role>>> Get([FromQuery] List<int> ids)
    {
        return await RoleService.Get(RoleService.DataContext.AccountRoles, ids);
    }

    /// <summary>
    ///     Получить роли по-умолчанию.
    /// </summary>
    /// <returns>Результат операции со списком ролей.</returns>
    [HttpGet("default")]
    public async Task<List<Role>> GetDefault()
    {
        return await RoleService.GetDefault();
    }

    /// <summary>
    ///     Получить список ролей по идентификаторам аккаунтов.
    /// </summary>
    /// <param name="ids">Идентификаторы аккаунтов.</param>
    /// <returns>Результат операции со списком ролей.</returns>
    /// <exception cref="ArgumentException">При пустом списке идентификаторов.</exception>
    [HttpGet("account")]
    public async Task<List<Role>> GetByAccount([FromQuery] List<int> ids)
    {
        return await RoleService.GetByAccount(ids);
    }

    /// <summary>
    ///     Получить список ролей по доступам.
    /// </summary>
    /// <param name="permissions">Доступы.</param>
    /// <returns>Результат операции со списком ролей.</returns>
    /// <exception cref="ArgumentException">При пустом списке доступов.</exception>
    [HttpGet("permission")]
    public async Task<List<Role>> GetByPermission([FromQuery] List<string> permissions)
    {
        return await RoleService.GetByPermission(permissions);
    }

    /// <summary>
    ///     Сохранить роли.
    /// </summary>
    /// <param name="entities">Список ролей.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<Role> entities)
    {
        var status = await RoleService.Set(RoleService.DataContext.AccountRoles, entities);

        if (!status)
        {
            return BadRequest("No roles were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить роли.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] List<int> ids)
    {
        var status = await RoleService.Remove(RoleService.DataContext.AccountRoles, ids);

        if (!status)
        {
            return BadRequest("No roles were deleted!");
        }

        return Ok();
    }
}
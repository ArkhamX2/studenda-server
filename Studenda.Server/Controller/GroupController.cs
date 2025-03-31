using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Common;
using Studenda.Server.Service;

namespace Studenda.Server.Controller;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="Group" />.
/// </summary>
/// <param name="dataEntityService">Сервис моделей.</param>
[Route(UpstreamConfiguration.Group)]
[ApiController]
public class GroupController(DataEntityService dataEntityService) : ControllerBase
{
    /// <summary>
    ///     Сервис моделей.
    /// </summary>
    private DataEntityService DataEntityService { get; } = dataEntityService;

    /// <summary>
    ///     Получить список групп.
    ///     Если идентификаторы не указаны, возвращается список со всеми группами.
    ///     Иначе возвращается список с указанными группами, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком групп.</returns>
    [HttpGet]
    public async Task<ActionResult<List<Group>>> Get([FromQuery] List<int> ids)
    {
        return await DataEntityService.Get(DataEntityService.DataContext.Groups, ids);
    }

    /// <summary>
    ///     Сохранить группы.
    /// </summary>
    /// <param name="entities">Список групп.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<Group> entities)
    {
        var status = await DataEntityService.Set(DataEntityService.DataContext.Groups, entities);

        if (!status)
        {
            return BadRequest("No groups were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить группы.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] List<int> ids)
    {
        var status = await DataEntityService.Remove(DataEntityService.DataContext.Groups, ids);

        if (!status)
        {
            return BadRequest("No groups were deleted!");
        }

        return Ok();
    }
}
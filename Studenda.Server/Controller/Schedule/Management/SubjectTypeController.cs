using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Schedule.Management;
using Studenda.Server.Service;

namespace Studenda.Server.Controller.Schedule.Management;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="SubjectType" />.
/// </summary>
/// <param name="dataEntityService">Сервис моделей.</param>
[Route(UpstreamConfiguration.ScheduleSubjectType)]
[ApiController]
public class SubjectTypeController(DataEntityService dataEntityService) : ControllerBase
{
    /// <summary>
    ///     Сервис моделей.
    /// </summary>
    private DataEntityService DataEntityService { get; } = dataEntityService;

    /// <summary>
    ///     Получить список типов занятия.
    ///     Если идентификаторы не указаны, возвращается список со всеми типами.
    ///     Иначе возвращается список с указанными типами, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком типов.</returns>
    [HttpGet]
    public async Task<ActionResult<List<SubjectType>>> Get([FromQuery] List<int> ids)
    {
        return await DataEntityService.Get(DataEntityService.DataContext.SubjectTypes, ids);
    }

    /// <summary>
    ///     Сохранить типы занятия.
    /// </summary>
    /// <param name="entities">Список типов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<SubjectType> entities)
    {
        var status = await DataEntityService.Set(DataEntityService.DataContext.SubjectTypes, entities);

        if (!status)
        {
            return BadRequest("No subject types were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить типы занятия.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] List<int> ids)
    {
        var status = await DataEntityService.Remove(DataEntityService.DataContext.SubjectTypes, ids);

        if (!status)
        {
            return BadRequest("No subject types were deleted!");
        }

        return Ok();
    }
}
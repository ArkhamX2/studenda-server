using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Schedule.Management;
using Studenda.Server.Service;

namespace Studenda.Server.Controller.Schedule.Management;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="SubjectPosition" />.
/// </summary>
/// <param name="dataEntityService">Сервис моделей.</param>
[Route(UpstreamConfiguration.ScheduleSubjectPosition)]
[ApiController]
public class SubjectPositionController(DataEntityService dataEntityService) : ControllerBase
{
    /// <summary>
    ///     Сервис моделей.
    /// </summary>
    private DataEntityService DataEntityService { get; } = dataEntityService;

    /// <summary>
    ///     Получить список позиций занятия.
    ///     Если идентификаторы не указаны, возвращается список со всеми позициями.
    ///     Иначе возвращается список с указанными позициями, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком позиций.</returns>
    [HttpGet]
    public async Task<ActionResult<List<SubjectPosition>>> Get([FromQuery] List<int> ids)
    {
        return await DataEntityService.Get(DataEntityService.DataContext.SubjectPositions, ids);
    }

    /// <summary>
    ///     Сохранить позиции занятия.
    /// </summary>
    /// <param name="entities">Список позиций.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<SubjectPosition> entities)
    {
        var status = await DataEntityService.Set(DataEntityService.DataContext.SubjectPositions, entities);

        if (!status)
        {
            return BadRequest("No subject positions were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить позиции занятия.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] List<int> ids)
    {
        var status = await DataEntityService.Remove(DataEntityService.DataContext.SubjectPositions, ids);

        if (!status)
        {
            return BadRequest("No subject positions were deleted!");
        }

        return Ok();
    }
}
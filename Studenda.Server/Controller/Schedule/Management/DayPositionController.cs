using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Schedule.Management;
using Studenda.Server.Service;

namespace Studenda.Server.Controller.Schedule.Management;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="DayPosition" />.
/// </summary>
/// <param name="dataEntityService">Сервис моделей.</param>
[Route(UpstreamConfiguration.ScheduleDayPosition)]
[ApiController]
public class DayPositionController(DataEntityService dataEntityService) : ControllerBase
{
    /// <summary>
    ///     Сервис моделей.
    /// </summary>
    private DataEntityService DataEntityService { get; } = dataEntityService;

    /// <summary>
    ///     Получить список позиций учебного дня.
    ///     Если идентификаторы не указаны, возвращается список со всеми позициями.
    ///     Иначе возвращается список с указанными позициями, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком позиций.</returns>
    [HttpGet]
    public async Task<ActionResult<List<DayPosition>>> Get([FromQuery] List<int> ids)
    {
        return await DataEntityService.Get(DataEntityService.DataContext.DayPositions, ids);
    }

    /// <summary>
    ///     Сохранить позиции учебного дня.
    /// </summary>
    /// <param name="entities">Список позиций.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<DayPosition> entities)
    {
        var status = await DataEntityService.Set(DataEntityService.DataContext.DayPositions, entities);

        if (!status)
        {
            return BadRequest("No day positions were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить позиции учебного дня.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] List<int> ids)
    {
        var status = await DataEntityService.Remove(DataEntityService.DataContext.DayPositions, ids);

        if (!status)
        {
            return BadRequest("No day positions were deleted!");
        }

        return Ok();
    }
}
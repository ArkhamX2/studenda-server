using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Schedule.Management;
using Studenda.Server.Service.Schedule;

namespace Studenda.Server.Controller.Schedule.Management;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="WeekType" />.
/// </summary>
/// <param name="weekTypeService">Сервис типов недель.</param>
[Route(UpstreamConfiguration.ScheduleWeekType)]
[ApiController]
public class WeekTypeController(WeekTypeService weekTypeService) : ControllerBase
{
    /// <summary>
    ///     Сервис моделей.
    /// </summary>
    private WeekTypeService WeekTypeService { get; } = weekTypeService;

    /// <summary>
    ///     Получить текущий тип недели.
    /// </summary>
    /// <returns>Результат операции с типом недели или пустой результат.</returns>
    [HttpGet("current")]
    public async Task<ActionResult<WeekType?>> GetCurrent()
    {
        return await WeekTypeService.GetCurrent();
    }

    /// <summary>
    ///     Получить список типов недель.
    ///     Если идентификаторы не указаны, возвращается список со всеми типами недель.
    ///     Иначе возвращается список с указанными типами недель.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком типов недель.</returns>
    [HttpGet]
    public async Task<ActionResult<List<WeekType>>> Get([FromQuery] List<int> ids)
    {
        return await WeekTypeService.Get(WeekTypeService.DataContext.WeekTypes, ids);
    }

    /// <summary>
    ///     Сохранить типы недель.
    /// </summary>
    /// <param name="entities">Список типов недель.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<WeekType> entities)
    {
        var status = await WeekTypeService.Set(entities);

        if (!status)
        {
            return BadRequest("No week types were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить типы недель.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] List<int> ids)
    {
        var status = await WeekTypeService.Remove(ids);

        if (!status)
        {
            return BadRequest("No week types were deleted!");
        }

        return Ok();
    }
}
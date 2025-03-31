using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Journal.Management;
using Studenda.Server.Service;

namespace Studenda.Server.Controller.Journal.Management;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="MarkType" />.
/// </summary>
/// <param name="dataEntityService">Сервис моделей.</param>
[Route(UpstreamConfiguration.JournalMarkType)]
[ApiController]
public class MarkTypeController(DataEntityService dataEntityService) : ControllerBase
{
    /// <summary>
    ///     Сервис моделей.
    /// </summary>
    private DataEntityService DataEntityService { get; } = dataEntityService;

    /// <summary>
    ///     Получить список типов оценивания.
    ///     Если идентификаторы не указаны, возвращается список со всеми типами.
    ///     Иначе возвращается список с указанными типами, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком типов оценивания.</returns>
    [HttpGet]
    public async Task<ActionResult<List<MarkType>>> Get([FromQuery] List<int> ids)
    {
        return await DataEntityService.Get(DataEntityService.DataContext.MarkTypes, ids);
    }

    /// <summary>
    ///     Сохранить типы оценивания.
    /// </summary>
    /// <param name="entities">Список типов оценивания.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = TeacherAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<MarkType> entities)
    {
        var status = await DataEntityService.Set(DataEntityService.DataContext.MarkTypes, entities);

        if (!status)
        {
            return BadRequest("No mark types were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить типы оценивания.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = TeacherAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] List<int> ids)
    {
        var status = await DataEntityService.Remove(DataEntityService.DataContext.MarkTypes, ids);

        if (!status)
        {
            return BadRequest("No mark types were deleted!");
        }

        return Ok();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Schedule.Management;
using Studenda.Server.Service;

namespace Studenda.Server.Controller.Schedule.Management;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="Discipline" />.
/// </summary>
/// <param name="dataEntityService">Сервис моделей.</param>
[Route(UpstreamConfiguration.ScheduleDiscipline)]
[ApiController]
public class DisciplineController(DataEntityService dataEntityService) : ControllerBase
{
    /// <summary>
    ///     Сервис моделей.
    /// </summary>
    private DataEntityService DataEntityService { get; } = dataEntityService;

    /// <summary>
    ///     Получить список дисциплин.
    ///     Если идентификаторы не указаны, возвращается список со всеми дисциплинами.
    ///     Иначе возвращается список с указанными дисциплинами, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком дисциплин.</returns>
    [HttpGet]
    public async Task<ActionResult<List<Discipline>>> Get([FromQuery] List<int> ids)
    {
        return await DataEntityService.Get(DataEntityService.DataContext.Disciplines, ids);
    }

    /// <summary>
    ///     Сохранить дисциплины.
    /// </summary>
    /// <param name="entities">Список дисциплин.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<Discipline> entities)
    {
        var status = await DataEntityService.Set(DataEntityService.DataContext.Disciplines, entities);

        if (!status)
        {
            return BadRequest("No disciplines were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить дисциплины.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] List<int> ids)
    {
        var status = await DataEntityService.Remove(DataEntityService.DataContext.Disciplines, ids);

        if (!status)
        {
            return BadRequest("No disciplines were deleted!");
        }

        return Ok();
    }
}
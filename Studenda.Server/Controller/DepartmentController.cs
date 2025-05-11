using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Common;
using Studenda.Server.Service;

namespace Studenda.Server.Controller;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="Department" />.
/// </summary>
/// <param name="dataEntityService">Сервис моделей.</param>
[Route(UpstreamConfiguration.Department)]
[ApiController]
public class DepartmentController(DataEntityService dataEntityService) : ControllerBase
{
    /// <summary>
    ///     Сервис моделей.
    /// </summary>
    private DataEntityService DataEntityService { get; } = dataEntityService;

    /// <summary>
    ///     Получить список факультетов.
    ///     Если идентификаторы не указаны, возвращается список со всеми факультетами.
    ///     Иначе возвращается список со списком факультетом, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком факультетов.</returns>
    [HttpGet]
    public async Task<ActionResult<List<Department>>> Get([FromQuery] List<int> ids)
    {
        return await DataEntityService.Get(DataEntityService.DataContext.Departments, ids);
    }

    /// <summary>
    ///     Сохранить факультеты.
    /// </summary>
    /// <param name="entities">Список факультетов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<Department> entities)
    {
        var status = await DataEntityService.Set(DataEntityService.DataContext.Departments, entities);

        if (!status)
        {
            return BadRequest("No departments were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить факультеты.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] List<int> ids)
    {
        var status = await DataEntityService.Remove(DataEntityService.DataContext.Departments, ids);

        if (!status)
        {
            return BadRequest("No departments were deleted!");
        }

        return Ok();
    }
}
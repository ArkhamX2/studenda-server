using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Common;
using Studenda.Server.Service;

namespace Studenda.Server.Controller;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="Course" />.
/// </summary>
/// <param name="dataEntityService">Сервис моделей.</param>
[Route(UpstreamConfiguration.Course)]
[ApiController]
public class CourseController(DataEntityService dataEntityService) : ControllerBase
{
    /// <summary>
    ///     Сервис моделей.
    /// </summary>
    private DataEntityService DataEntityService { get; } = dataEntityService;

    /// <summary>
    ///     Получить список курсов.
    ///     Если идентификаторы не указаны, возвращается список со всеми курсами.
    ///     Иначе возвращается список с указанными курсами, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком курсов.</returns>
    [HttpGet]
    public async Task<ActionResult<List<Course>>> Get([FromQuery] List<int> ids)
    {
        return await DataEntityService.Get(DataEntityService.DataContext.Courses, ids);
    }

    /// <summary>
    ///     Сохранить курсы.
    /// </summary>
    /// <param name="entities">Список курсов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<Course> entities)
    {
        var status = await DataEntityService.Set(DataEntityService.DataContext.Courses, entities);

        if (!status)
        {
            return BadRequest("No courses were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить курсы.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] List<int> ids)
    {
        var status = await DataEntityService.Remove(DataEntityService.DataContext.Courses, ids);

        if (!status)
        {
            return BadRequest("No courses were deleted!");
        }

        return Ok();
    }
}
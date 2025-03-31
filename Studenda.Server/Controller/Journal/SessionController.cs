using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Journal;
using Studenda.Server.Service.Journal;

namespace Studenda.Server.Controller.Journal;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="Session" />.
/// </summary>
/// <param name="sessionService">Сервис моделей.</param>
[Route(UpstreamConfiguration.JournalSession)]
[ApiController]
public class SessionController(SessionService sessionService) : ControllerBase
{
    /// <summary>
    ///     Сервис моделей.
    /// </summary>
    private SessionService SessionService { get; } = sessionService;

    /// <summary>
    ///     Получить список учебных сессий.
    ///     Если идентификаторы не указаны, возвращается список со всеми учебными сессиями.
    ///     Иначе возвращается список с указанными учебными сессиями, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком учебных сессий.</returns>
    [HttpGet]
    public async Task<ActionResult<List<Session>>> Get([FromQuery] List<int> ids)
    {
        return await SessionService.Get(SessionService.DataContext.Sessions, ids);
    }

    /// <summary>
    ///     Получить список учебных сессий по идентификатору занятия.
    /// </summary>
    /// <param name="subjectId">Идентификатор занятия.</param>
    /// <param name="dates">Даты.</param>
    /// <returns>Результат операции со списком учебных сессий.</returns>
    [HttpGet]
    [Route("subject")]
    public async Task<ActionResult<List<Session>>> GetBySubject([FromQuery] int subjectId, [FromQuery] List<DateTime> dates)
    {
        return await SessionService.GetBySubject(subjectId, dates);
    }

    /// <summary>
    ///     Получить список учебных сессий по датам.
    /// </summary>
    /// <param name="subjectIds">Идентификаторы занятий.</param>
    /// <param name="date">Дата.</param>
    /// <returns>Результат операции со списком учебных сессий.</returns>
    [HttpGet]
    [Route("date")]
    public async Task<ActionResult<List<Session>>> GetByDate([FromQuery] List<int> subjectIds, [FromQuery] DateTime date)
    {
        return await SessionService.GetByDate(subjectIds, date);
    }

    /// <summary>
    ///     Сохранить учебные сессии.
    /// </summary>
    /// <param name="entities">Список учебных сессий.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = TeacherAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<Session> entities)
    {
        var status = await SessionService.Set(SessionService.DataContext.Sessions, entities);

        if (!status)
        {
            return BadRequest("No sessions were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить учебные сессии.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = TeacherAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] List<int> ids)
    {
        var status = await SessionService.Remove(SessionService.DataContext.Sessions, ids);

        if (!status)
        {
            return BadRequest("No sessions were deleted!");
        }

        return Ok();
    }
}
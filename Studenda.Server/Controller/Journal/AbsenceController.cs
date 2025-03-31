using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Journal;
using Studenda.Server.Service.Journal;

namespace Studenda.Server.Controller.Journal;

/// <summary>
///     Контроллер для работы с объектами типа <see cref="Absence" />.
/// </summary>
/// <param name="absenceService">Сервис моделей.</param>
[Route(UpstreamConfiguration.JournalAbsence)]
[ApiController]
public class AbsenceController(AbsenceService absenceService) : ControllerBase
{
    /// <summary>
    ///     Сервис моделей.
    /// </summary>
    private AbsenceService AbsenceService { get; } = absenceService;

    /// <summary>
    ///     Получить список прогулов.
    ///     Если идентификаторы не указаны, возвращается список со всеми прогулами.
    ///     Иначе возвращается список с указанными прогулами, либо пустой список.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции со списком прогулов.</returns>
    [HttpGet]
    public async Task<ActionResult<List<Absence>>> Get([FromQuery] List<int> ids)
    {
        return await AbsenceService.Get(AbsenceService.DataContext.Absences, ids);
    }

    /// <summary>
    ///     Получить список прогулов по идентификатору аккаунта.
    /// </summary>
    /// <param name="accountId">Идентификатор аккаунта.</param>
    /// <param name="sessionIds">Идентификаторы учебных сессий.</param>
    /// <returns>Результат операции со списком прогулов.</returns>
    [HttpGet]
    [Route("account")]
    public async Task<ActionResult<List<Absence>>> GetByAccount([FromQuery] int accountId, [FromQuery] List<int> sessionIds)
    {
        return await AbsenceService.GetByAccount(accountId, sessionIds);
    }

    /// <summary>
    ///     Получить список прогулов по идентификаторам учебных сессий.
    /// </summary>
    /// <param name="accountIds">Идентификаторы аккаунтов.</param>
    /// <param name="sessionId">Идентификатор учебной сессии.</param>
    /// <returns>Результат операции со списком прогулов.</returns>
    [HttpGet]
    [Route("session")]
    public async Task<ActionResult<List<Absence>>> GetBySession([FromQuery] List<int> accountIds, [FromQuery] int sessionId)
    {
        return await AbsenceService.GetBySession(accountIds, sessionId);
    }

    /// <summary>
    ///     Сохранить прогулы.
    /// </summary>
    /// <param name="entities">Список прогулов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = TeacherAuthorizationRequirement.PolicyCode)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<Absence> entities)
    {
        var status = await AbsenceService.Set(AbsenceService.DataContext.Absences, entities);

        if (!status)
        {
            return BadRequest("No absences were saved!");
        }

        return Ok();
    }

    /// <summary>
    ///     Удалить прогулы.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Результат операции.</returns>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] List<int> ids)
    {
        var status = await AbsenceService.Remove(AbsenceService.DataContext.Absences, ids);

        if (!status)
        {
            return BadRequest("No absences were deleted!");
        }

        return Ok();
    }
}
using Microsoft.EntityFrameworkCore;
using Studenda.Server.Data;
using Studenda.Server.Model.Journal;

namespace Studenda.Server.Service.Journal;

/// <summary>
///     Сервис для работы с <see cref="Session" />.
/// </summary>
/// <param name="dataContext">Контекст данных.</param>
public class SessionService(DataContext dataContext) : DataEntityService(dataContext)
{
    /// <summary>
    ///     Получить список учебных сессий по идентификатору занятия.
    /// </summary>
    /// <param name="subjectId">Идентификатор занятия.</param>
    /// <param name="dates">Даты.</param>
    /// <returns>Список учебных сессий.</returns>
    /// <exception cref="ArgumentException">При некорректных аргументах.</exception>
    public async Task<List<Session>> GetBySubject(int subjectId, List<DateTime> dates)
    {
        if (subjectId <= 0)
        {
            throw new ArgumentException("Invalid subject id!");
        }

        if (dates.Count <= 0)
        {
            throw new ArgumentException("Invalid dates!");
        }

        var datesHashSet = new HashSet<DateTime>(dates.Select(date => new DateTime(date.Year, date.Month, date.Day)));

        return await DataContext.Sessions
            .Where(session => session.SubjectId == subjectId
                && session.StartedAt != null
                && datesHashSet.Contains(session.StartedAt.Value.Date))
            .ToListAsync();
    }

    /// <summary>
    ///     Получить список учебных сессий по датам.
    /// </summary>
    /// <param name="subjectIds">Идентификаторы учебных сессий.</param>
    /// <param name="date">Дата.</param>
    /// <returns>Список учебных сессий.</returns>
    /// <exception cref="ArgumentException">При пустом списке идентификаторов учебных сессий.</exception>
    public async Task<List<Session>> GetByDate(List<int> subjectIds, DateTime date)
    {
        if (subjectIds.Count <= 0)
        {
            throw new ArgumentException("Invalid subject ids!");
        }

        return await DataContext.Sessions
            .Where(session => subjectIds.Contains(session.SubjectId)
                && session.StartedAt != null
                && session.StartedAt.Value.Date == date.Date)
            .ToListAsync();
    }
}
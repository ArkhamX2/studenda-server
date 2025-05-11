using Microsoft.EntityFrameworkCore;
using Studenda.Server.Data;
using Studenda.Server.Model.Schedule;

namespace Studenda.Server.Service.Schedule;

/// <summary>
///     Сервис для работы с <see cref="Subject" />.
/// </summary>
/// <param name="dataContext">Контекст данных.</param>
public class SubjectService(DataContext dataContext) : DataEntityService(dataContext)
{
    /// <summary>
    ///     Получить список статичных занятий по идентификатору группы.
    /// </summary>
    /// <param name="groupId">Идентификатор группы.</param>
    /// <param name="year">Учебный год.</param>
    /// <returns>Список статичных занятий.</returns>
    public async Task<List<Subject>> GetAllByGroup(int groupId, int year)
    {
        if (groupId <= 0)
        {
            throw new ArgumentException("Invalid arguments!");
        }

        return await DataContext.Subjects
            .Where(subject => subject.GroupId == groupId
                              && subject.AcademicYear == year)
            .OrderBy(subject => subject.CreatedAt)
            .ThenBy(subject => subject.DayPosition!.Index)
            .ThenBy(subject => subject.SubjectPosition!.Index)
            .ToListAsync();
    }

    /// <summary>
    ///     Получить список статичных занятий по идентификатору группы.
    /// </summary>
    /// <param name="groupId">Идентификатор группы.</param>
    /// <param name="weekTypeId">Идентификатор типа недели.</param>
    /// <param name="year">Учебный год.</param>
    /// <returns>Список статичных занятий.</returns>
    public async Task<List<Subject>> GetByGroup(int groupId, int weekTypeId, int year)
    {
        if (groupId <= 0 || weekTypeId <= 0)
        {
            throw new ArgumentException("Invalid arguments!");
        }

        return await DataContext.Subjects
            .Where(subject => subject.GroupId == groupId
                              && subject.WeekTypeId == weekTypeId
                              && subject.AcademicYear == year)
            .OrderBy(subject => subject.CreatedAt)
            .ThenBy(subject => subject.DayPosition!.Index)
            .ThenBy(subject => subject.SubjectPosition!.Index)
            .ToListAsync();
    }

    /// <summary>
    ///     Получить список статичных занятий по идентификатору пользователя.
    /// </summary>
    /// <param name="accountId">Идентификатор пользователя.</param>
    /// <param name="year">Учебный год.</param>
    /// <returns>Список статичных занятий.</returns>
    public async Task<List<Subject>> GetAllByAccount(int accountId, int year)
    {
        if (accountId <= 0)
        {
            throw new ArgumentException("Invalid arguments!");
        }

        return await DataContext.Subjects
            .Where(subject => subject.AccountId == accountId
                              && subject.AcademicYear == year)
            .OrderBy(subject => subject.CreatedAt)
            .ThenBy(subject => subject.DayPosition!.Index)
            .ThenBy(subject => subject.SubjectPosition!.Index)
            .ToListAsync();
    }

    /// <summary>
    ///     Получить список статичных занятий по идентификатору пользователя.
    /// </summary>
    /// <param name="accountId">Идентификатор пользователя.</param>
    /// <param name="weekTypeId">Идентификатор типа недели.</param>
    /// <param name="year">Учебный год.</param>
    /// <returns>Список статичных занятий.</returns>
    public async Task<List<Subject>> GetByAccount(int accountId, int weekTypeId, int year)
    {
        if (accountId <= 0 || weekTypeId <= 0)
        {
            throw new ArgumentException("Invalid arguments!");
        }

        return await DataContext.Subjects
            .Where(subject => subject.AccountId == accountId
                              && subject.WeekTypeId == weekTypeId
                              && subject.AcademicYear == year)
            .OrderBy(subject => subject.CreatedAt)
            .ThenBy(subject => subject.DayPosition!.Index)
            .ThenBy(subject => subject.SubjectPosition!.Index)
            .ToListAsync();
    }
}
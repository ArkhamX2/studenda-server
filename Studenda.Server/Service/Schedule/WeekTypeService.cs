using Microsoft.EntityFrameworkCore;
using Studenda.Server.Data;
using Studenda.Server.Model.Schedule.Management;

namespace Studenda.Server.Service.Schedule;

/// <summary>
///     Сервис для работы с <see cref="WeekType" />.
/// </summary>
/// <param name="dataContext">Контекст данных.</param>
public class WeekTypeService(DataContext dataContext) : DataEntityService(dataContext)
{
    /// <summary>
    ///    Месяц начала учебного года.
    ///    TODO: В отдельный класс.
    /// </summary>
    private const int AcademicYearStartMonth = 9;

    /// <summary>
    ///    День начала учебного года.
    ///    TODO: В отдельный класс.
    /// </summary>
    private const int AcademicYearStartDay = 1;

    /// <summary>
    ///     Получить текущий тип недели.
    /// </summary>
    /// <returns>Тип недели или ничего.</returns>
    public async Task<WeekType?> GetCurrent()
    {
        var today = DateTime.Now;
        var startOfAcademicYear = new DateTime(today.Year, AcademicYearStartMonth, AcademicYearStartDay);

        if (today < startOfAcademicYear)
        {
            startOfAcademicYear = startOfAcademicYear.AddYears(-1);
        }

        // Первый понедельник начала учебного года
        while (startOfAcademicYear.DayOfWeek != DayOfWeek.Monday)
        {
            startOfAcademicYear = startOfAcademicYear.AddDays(1);
        }

        // Понедельник текущей недели
        var currentMonday = today;

        while (currentMonday.DayOfWeek != DayOfWeek.Monday)
        {
            currentMonday = currentMonday.AddDays(-1);
        }

        var weeksPassed = Math.Ceiling((currentMonday - startOfAcademicYear).TotalDays / 7) + 1;
        var maxIndex = await DataContext.WeekTypes.MaxAsync(type => type.Index);
        var circularIndex = (int)weeksPassed % maxIndex;

        if (circularIndex == 0)
        {
            circularIndex = maxIndex;
        }

        return await DataContext.WeekTypes.FirstOrDefaultAsync(type => type.Index == circularIndex);
    }

    /// <summary>
    ///     Задать список типов недели.
    /// </summary>
    /// <param name="weekTypes">Список типов недели.</param>
    /// <returns>Статус операции.</returns>
    public async Task<bool> Set(List<WeekType> weekTypes)
    {
        if (weekTypes.Count <= 0)
        {
            return false;
        }

        var minIndex = weekTypes.Min(type => type.Index);

        if (await DataContext.WeekTypes.AnyAsync())
        {
            var maxIndex = await DataContext.WeekTypes.MaxAsync(type => type.Index);

            if (minIndex - maxIndex > 1)
            {
                throw new ArgumentException("Indexes must be sequential!");
            }
        }
        else if (minIndex != WeekType.StartIndex)
        {
            throw new ArgumentException($"Indexes must start from {WeekType.StartIndex}!");
        }

        var indexes = weekTypes.Select(type => type.Index).ToList();
        var weekTypesToRemove = await DataContext.WeekTypes.Where(type => indexes.Contains(type.Index)).ToListAsync();

        if (weekTypesToRemove.Any())
        {
            DataContext.WeekTypes.RemoveRange(weekTypesToRemove);
            DataContext.SaveChanges();
        }

        return await base.Set(DataContext.WeekTypes, weekTypes);
    }

    /// <summary>
    ///     Удалить список типов недели.
    ///     Происходит переиндексация типов недель.
    /// </summary>
    /// <param name="ids">Список идентификаторов.</param>
    /// <returns>Статус операции.</returns>
    public async Task<bool> Remove(List<int> ids)
    {
        var baseResult = await base.Remove(DataContext.WeekTypes, ids);

        if (!baseResult)
        {
            return false;
        }

        var remainingWeekTypes = await DataContext.WeekTypes.OrderBy(type => type.Index).ToListAsync();

        // Обновление индексов
        for (var i = 0; i < remainingWeekTypes.Count; i++)
        {
            remainingWeekTypes[i].Index = i;
        }

        DataContext.WeekTypes.UpdateRange(remainingWeekTypes);
        await DataContext.SaveChangesAsync();

        return true;
    }
}
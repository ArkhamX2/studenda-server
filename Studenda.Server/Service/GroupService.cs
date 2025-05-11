using Microsoft.EntityFrameworkCore;
using Studenda.Server.Data;
using Studenda.Server.Model.Common;

namespace Studenda.Server.Service.Schedule;

/// <summary>
///     Сервис для работы с <see cref="Group" />.
/// </summary>
/// <param name="dataContext">Контекст данных.</param>
public class GroupService(DataContext dataContext) : DataEntityService(dataContext)
{
    /// <summary>
    ///     Получить список групп по идентификатору факультета.
    /// </summary>
    /// <param name="departmentId">Идентификатор факультета.</param>
    /// <returns>Список групп.</returns>
    public async Task<List<Group>> GetByDepartment(int departmentId)
    {
        if (departmentId <= 0)
        {
            throw new ArgumentException("Invalid arguments!");
        }

        return await DataContext.Groups
            .Where(group => group.DepartmentId == departmentId)
            .OrderBy(group => group.CreatedAt)
            .ToListAsync();
    }
}
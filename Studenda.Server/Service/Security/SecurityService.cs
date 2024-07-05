using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Studenda.Server.Configuration.Repository;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Data;
using Studenda.Server.Model.Security;
using ConfigurationManager = Studenda.Server.Configuration.ConfigurationManager;

namespace Studenda.Server.Service.Security;

/// <summary>
///     Сервис работы с безопасностью.
/// </summary>
/// <param name="configurationManager">Менеджер конфигурации.</param>
/// <param name="dataContext">Контекст данных.</param>
/// <param name="accountService">Сервис работы с ролями.</param>
/// <param name="userManager">Менеджер пользователей.</param>
public class SecurityService(
    ConfigurationManager configurationManager,
    DataContext dataContext,
    RoleService roleService,
    UserManager<IdentityUser> userManager
)
{
    private SecurityConfiguration SecurityConfiguration { get; } = configurationManager.SecurityConfiguration;
    private DataContext DataContext { get; } = dataContext;
    private RoleService RoleService { get; } = roleService;
    private UserManager<IdentityUser> UserManager { get; } = userManager;

    /// <summary>
    ///     Создать пользователя по-умолчанию.
    /// </summary>
    /// <returns>Операция.</returns>
    /// <exception cref="Exception">При ошибке создания.</exception>
    public async Task CreateDefaultUser()
    {
        var roleName = SecurityConfiguration.GetDefaultRoleName();
        var rolePermission = SecurityConfiguration.GetDefaultRolePermission();

        if (!await DataContext.AccountRoles.AnyAsync(role => role.Permission == rolePermission))
        {
            await DataContext.AccountRoles.AddAsync(new Role
            {
                Name = roleName,
                Permission = rolePermission,
                TokenLifetimeSeconds = Role.DefaultTokenLifetimeSeconds,
                CanRegister = Role.DefaultCanRegister
            });

            await DataContext.SaveChangesAsync();
        }

        var roleId = await DataContext.AccountRoles.Where(role => role.Permission == rolePermission)
            .Select(role => role.Id)
            .FirstAsync();

        if (!roleId.HasValue)
        {
            throw new Exception("Data initialization failed while creating default role!");
        }

        var userEmail = SecurityConfiguration.GetDefaultUserEmail();
        var userPassword = SecurityConfiguration.GetDefaultUserPassword();
        var user = await DataContext.Users
            .FirstOrDefaultAsync(user => user.Email == userEmail);

        if (user is null)
        {
            var result = await UserManager.CreateAsync(
                new IdentityUser
                {
                    UserName = userEmail,
                    Email = userEmail
                },
                userPassword);

            if (!result.Succeeded)
            {
                throw new Exception("Data initialization failed while creating default user!");
            }

            user = await DataContext.Users
                .FirstOrDefaultAsync(user => user.Email == userEmail);
        }

        if (user is null)
        {
            throw new Exception("Data initialization failed while creating default user!");
        }

        var accountName = SecurityConfiguration.GetDefaultAccountName();
        var accountSurname = SecurityConfiguration.GetDefaultAccountSurname();
        var accountPatronymic = SecurityConfiguration.GetDefaultAccountPatronymic();

        if (!await DataContext.Accounts.AnyAsync(account => account.IdentityId == user.Id))
        {
            await DataContext.Accounts.AddAsync(new Account
            {
                RoleId = roleId.GetValueOrDefault(),
                IdentityId = user.Id,
                Email = userEmail,
                Name = accountName,
                Surname = accountSurname,
                Patronymic = accountPatronymic
            });

            await DataContext.SaveChangesAsync();
        }
    }

    /// <summary>
    ///     Проверить возможность регистрации с указанным доступом.
    /// </summary>
    /// <param name="permission">Доступ.</param>
    /// <returns>Статус проверки.</returns>
    public async Task<bool> CanRegisterWithPermission(string permission)
    {
        if (!PermissionConfiguration.GetPermissions().Contains(permission))
        {
            return false;
        }

        var roles = await RoleService.GetDefault();
        var role = roles.FirstOrDefault();

        if (role is null)
        {
            return false;
        }

        return permission == role.Permission;
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studenda.Server.Configuration.Static;
using Studenda.Server.Data;
using Studenda.Server.Data.Transfer.Security;
using Studenda.Server.Data.Util;
using Studenda.Server.Middleware.Security.Requirement;
using Studenda.Server.Model.Security;
using Studenda.Server.Service.Security;

namespace Studenda.Server.Controller.Security;

/// <summary>
///     Контроллер авторизации аккаунтов.
/// </summary>
/// <param name="dataContext">Сессия работы с базой данных.</param>
/// <param name="tokenService">Сервис работы с токенами.</param>
/// <param name="accountService">Сервис работы с аккаунтами.</param>
/// <param name="roleService">Сервис работы с ролями.</param>
/// <param name="securityService">Сервис работы с безопасностью.</param>
/// <param name="userManager">Менеджер работы с пользователями.</param>
[Route("api/security")]
[ApiController]
public class SecurityController(
    DataContext dataContext,
    TokenService tokenService,
    AccountService accountService,
    RoleService roleService,
    SecurityService securityService,
    UserManager<IdentityUser> userManager
) : ControllerBase
{
    private DataContext DataContext { get; } = dataContext;
    private TokenService TokenService { get; } = tokenService;
    private AccountService AccountService { get; } = accountService;
    private RoleService RoleService { get; } = roleService;
    private SecurityService SecurityService { get; } = securityService;
    private UserManager<IdentityUser> UserManager { get; } = userManager;

    /// <summary>
    ///     Получить базовые параметры соединения.
    /// </summary>
    /// <returns>Результат с параметрами.</returns>
    [HttpGet]
    public ActionResult<List<Account>> GetSettings()
    {
        return Ok(new HandshakeResponse
        {
            DefaultPermission = PermissionConfiguration.DefaultPermission,
            LeaderPermission = PermissionConfiguration.LeaderPermission,
            TeacherPermission = PermissionConfiguration.TeacherPermission,
            AdminPermission = PermissionConfiguration.AdminPermission,
            CoordinatedUniversalTime = DateTime.UtcNow
        });
    }

    /// <summary>
    ///     Авторизовать пользователя.
    /// </summary>
    /// <param name="request">Тело запроса авторизации.</param>
    /// <returns>Тело ответа модуля безопасности.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<SecurityResponse>> AuthorizeUser(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(request);
        }

        var user = await UserManager.FindByEmailAsync(request.Email);

        if (user == null || !await UserManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized();
        }

        var accounts = await AccountService.GetByIdentityId([user.Id]);
        var accountIds = accounts.Select(x => x.Id.GetValueOrDefault()).ToList();
        var roles = await RoleService.GetByAccount(accountIds);

        var account = accounts.First();
        var role = roles.First();

        if (account is null || role is null)
        {
            return Unauthorized();
        }

        return Ok(DataSerializer.Serialize(new SecurityResponse
        {
            Account = account,
            Token = TokenService.CreateNewToken(user, role)
        }));
    }

    /// <summary>
    ///     Зарегистрировать нового пользователя.
    /// </summary>
    /// <param name="request">Тело запроса регистрации.</param>
    /// <returns>Тело ответа модуля безопасности.</returns>
    /// <exception cref="Exception">При неудачной попытке создания пользователя.</exception>
    [HttpPost("register")]
    public async Task<ActionResult<SecurityResponse>> RegisterNewUser([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(request);
        }

        var roles = await RoleService.Get(RoleService.DataContext.AccountRoles, [request.Account.RoleId]);
        var role = roles.FirstOrDefault();

        if (role is null)
        {
            return BadRequest("Incorrect permission! Role not found!");
        }

        var canRegister = await SecurityService.CanRegisterWithPermission(role.Permission);

        if (!canRegister)
        {
            return BadRequest("Incorrect permission!");
        }

        return await CreateNewUserInternal(request, role);
    }

    /// <summary>
    ///     Создать нового пользователя.
    /// </summary>
    /// <param name="request">Тело запроса регистрации.</param>
    /// <returns>Тело ответа модуля безопасности.</returns>
    /// <exception cref="Exception">При неудачной попытке создания пользователя.</exception>
    [Authorize(Policy = AdminAuthorizationRequirement.PolicyCode)]
    [HttpPost("user")]
    public async Task<ActionResult<SecurityResponse>> CreateNewUser([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(request);
        }

        var roles = await RoleService.Get(RoleService.DataContext.AccountRoles, [request.Account.RoleId]);
        var role = roles.FirstOrDefault();

        if (role is null)
        {
            return BadRequest("Incorrect permission! Role not found!");
        }

        return await CreateNewUserInternal(request, role);
    }

    /// <summary>
    ///     Выпустить новый токен для текущего авторизованного пользователя.
    /// </summary>
    /// <returns>Тело ответа модуля безопасности.</returns>
    [Authorize(Policy = DefaultAuthorizationRequirement.PolicyCode)]
    [HttpPost("token")]
    public async Task<IActionResult> RefreshToken()
    {
        if (User.Identity is null || !User.Identity.IsAuthenticated)
        {
            return Unauthorized();
        }

        var user = await DataContext.Users
            .FirstOrDefaultAsync(user => user.UserName == User.Identity.Name);

        if (user is null)
        {
            return Unauthorized();
        }

        var accounts = await AccountService.GetByIdentityId([user.Id]);
        var accountIds = accounts.Select(x => x.Id.GetValueOrDefault()).ToList();
        var roles = await RoleService.GetByAccount(accountIds);

        var account = accounts.First();
        var role = roles.First();

        if (account is null || role is null)
        {
            return Unauthorized();
        }

        return Ok(DataSerializer.Serialize(new SecurityResponse
        {
            Account = account,
            Token = TokenService.CreateNewToken(user, role)
        }));
    }

    /// <summary>
    ///     Создать нового пользователя.
    /// </summary>
    /// <param name="request">Тело запроса регистрации.</param>
    /// <returns>Тело ответа модуля безопасности.</returns>
    /// <exception cref="Exception">При неудачной попытке создания пользователя.</exception>
    private async Task<ActionResult<SecurityResponse>> CreateNewUserInternal(
        [FromBody] RegisterRequest request,
        Role role
    )
    {
        var result = await UserManager.CreateAsync(
            new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            },
            request.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        var user = await DataContext.Users.FirstOrDefaultAsync(user => user.Email == request.Email);

        if (user is null)
        {
            throw new Exception("Error while creating new identity user!");
        }

        var status = await AccountService.Set([
            new Account
            {
                RoleId = role.Id.GetValueOrDefault(),
                IdentityId = user.Id,
                GroupId = request.Account?.GroupId,
                Email = request.Email,
                Name = request.Account?.Name,
                Surname = request.Account?.Surname,
                Patronymic = request.Account?.Patronymic
            }
        ]);

        if (!status)
        {
            throw new Exception("Error while creating new account!");
        }

        var accounts = await AccountService.GetByIdentityId([user.Id]);
        var account = accounts.First();

        if (account is null)
        {
            return Unauthorized();
        }

        return Ok(DataSerializer.Serialize(new SecurityResponse
        {
            Account = account,
            Token = TokenService.CreateNewToken(user, role)
        }));
    }
}
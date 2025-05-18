using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studenda.Server.Data.Configuration;
using Studenda.Server.Model.Common;
using Studenda.Server.Model.Journal;
using Studenda.Server.Model.Schedule;
using Studenda.Server.Model.Schedule.Management;
using Task = Studenda.Server.Model.Journal.Task;

namespace Studenda.Server.Model.Security;

/// <summary>
///     Аккаунт пользователя.
/// </summary>
public class Account : IdentifiableEntity
{
    /*                   __ _                       _   _
     *   ___ ___  _ __  / _(_) __ _ _   _ _ __ __ _| |_(_) ___  _ __
     *  / __/ _ \| '_ \| |_| |/ _` | | | | '__/ _` | __| |/ _ \| '_ \
     * | (_| (_) | | | |  _| | (_| | |_| | | | (_| | |_| | (_) | | | |
     *  \___\___/|_| |_|_| |_|\__, |\__,_|_|  \__,_|\__|_|\___/|_| |_|
     *                        |___/
     * Константы, задающие базовые конфигурации полей
     * и ограничения модели.
     */

    #region Configuration

    public const int IdentityIdLengthMax = 128;
    public const int EmailLengthMax = 64;
    public const int NameLengthMax = 32;
    public const int SurnameLengthMax = 32;
    public const int PatronymicLengthMax = 32;
    public const int ParentEmailLengthMax = 64;
    public const bool IsRoleIdRequired = true;
    public const bool IsGroupIdRequired = false;
    public const bool IsIdentityIdRequired = false;
    public const bool IsEmailRequired = true;
    public const bool IsNameRequired = false;
    public const bool IsSurnameRequired = false;
    public const bool IsPatronymicRequired = false;
    public const bool IsParentEmailRequired = false;

    /// <summary>
    ///     Конфигурация модели <see cref="Account" />.
    /// </summary>
    /// <param name="configuration">Конфигурация базы данных.</param>
    internal class Configuration(ContextConfiguration configuration) : Configuration<Account>(configuration)
    {
        /// <summary>
        ///     Задать конфигурацию для модели.
        /// </summary>
        /// <param name="builder">Набор интерфейсов настройки модели.</param>
        public override void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasOne(account => account.Role)
                .WithMany(role => role.Accounts)
                .HasForeignKey(account => account.RoleId)
                .IsRequired();

            builder.HasOne(account => account.Group)
                .WithMany(group => group.Accounts)
                .HasForeignKey(account => account.GroupId)
                .IsRequired(IsGroupIdRequired);

            builder.Property(account => account.IdentityId)
                .HasMaxLength(IdentityIdLengthMax)
                .IsRequired(IsIdentityIdRequired);

            builder.Property(account => account.Email)
                .HasMaxLength(EmailLengthMax)
                .IsRequired();

            builder.Property(account => account.Name)
                .HasMaxLength(NameLengthMax)
                .IsRequired(IsNameRequired);

            builder.Property(account => account.Surname)
                .HasMaxLength(SurnameLengthMax)
                .IsRequired(IsSurnameRequired);

            builder.Property(account => account.Patronymic)
                .HasMaxLength(PatronymicLengthMax)
                .IsRequired(IsPatronymicRequired);

            builder.Property(account => account.ParentEmail)
                .HasMaxLength(ParentEmailLengthMax)
                .IsRequired(IsParentEmailRequired);

            builder.Property(account => account.MustChangePassword)
                .HasDefaultValue(false)
                .IsRequired();

            base.Configure(builder);
        }
    }

    #endregion

    /*             _   _ _
     *   ___ _ __ | |_(_) |_ _   _
     *  / _ \ '_ \| __| | __| | | |
     * |  __/ | | | |_| | |_| |_| |
     *  \___|_| |_|\__|_|\__|\__, |
     *                       |___/
     * Поля данных, соответствующие таковым в таблице
     * модели в базе данных.
     */

    #region Entity

    /// <summary>
    ///     Идентификатор связанного объекта <see cref="Security.Role" />.
    /// </summary>
    public required int RoleId { get; set; }

    /// <summary>
    ///     Идентификатор связанного объекта <see cref="Common.Group" />.
    ///     Необязательное поле.
    /// </summary>
    public int? GroupId { get; set; }

    /// <summary>
    ///     Идентификатор в системе безопасности.
    ///     Необязательное поле.
    /// </summary>
    public string? IdentityId { get; set; }

    /// <summary>
    ///     Почта.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    ///     Имя.
    ///     Необязательное поле.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Фамилия.
    ///     Необязательное поле.
    /// </summary>
    public string? Surname { get; set; }

    /// <summary>
    ///     Отчество.
    ///     Необязательное поле.
    /// </summary>
    public string? Patronymic { get; set; }

    /// <summary>
    ///     Электронная почта родителя.
    ///     Необязательное поле.
    /// </summary>
    public string? ParentEmail { get; set; }

    /// <summary>
    ///     Требуется ли смена пароля при следующем входе.
    /// </summary>
    public bool MustChangePassword { get; set; } = false;

    #endregion

    public Role? Role { get; set; } = null;
    public Group? Group { get; set; } = null;
    public List<Subject> Subjects { get; set; } = [];
    public List<SubjectChange> SubjectChanges { get; set; } = [];
    public List<Discipline> Disciplines { get; set; } = [];
    public List<Absence> Absences { get; set; } = [];
    public List<Task> IssuedTasks { get; set; } = [];
    public List<Task> AssignedTasks { get; set; } = [];
}
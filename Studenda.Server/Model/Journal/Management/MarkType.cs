using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studenda.Server.Data.Configuration;

namespace Studenda.Server.Model.Journal.Management;

/// <summary>
///     Тип оценивания.
/// </summary>
public class MarkType : IdentifiableEntity
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

    public const int NameLengthMax = 32;
    public const bool IsNameRequired = true;
    public const bool IsMinValueRequired = true;
    public const bool IsMaxValueRequired = true;

    /// <summary>
    ///     Конфигурация модели.
    /// </summary>
    /// <param name="configuration">Конфигурация базы данных.</param>
    internal class Configuration(ContextConfiguration configuration) : Configuration<MarkType>(configuration)
    {
        /// <summary>
        ///     Задать конфигурацию для модели.
        /// </summary>
        /// <param name="builder">Набор интерфейсов настройки модели.</param>
        public override void Configure(EntityTypeBuilder<MarkType> builder)
        {
            builder.Property(type => type.Name)
                .HasMaxLength(NameLengthMax)
                .IsRequired();

            builder.Property(type => type.MinValue)
                .IsRequired();

            builder.Property(type => type.MaxValue)
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
    ///     Название.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Минимальное значение оценки.
    /// </summary>
    public required int MinValue { get; set; }

    /// <summary>
    ///     Максимальное значение оценки.
    /// </summary>
    public required int MaxValue { get; set; }

    #endregion

    public List<Task> Tasks { get; set; } = [];
}
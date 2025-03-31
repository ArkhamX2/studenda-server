namespace Studenda.Server.Configuration.Static;

/// <summary>
///     Статичная конфигурация адресов.
/// </summary>
public static class UpstreamConfiguration
{
    /*
     * Общее.
     */

    public const string Department = "department";
    public const string Course = "course";
    public const string Group = "group";

    /*
     * Безопасность.
     */

    public const string Security = "security";
    public const string SecurityAccount = Security + "/account";
    public const string SecurityRole = Security + "/role";

    /*
     * Расписание.
     */

    public const string Schedule = "schedule";
    public const string ScheduleSubject = Schedule + "/subject";
    public const string ScheduleDayPosition = Schedule + "/day-position";
    public const string ScheduleDiscipline = Schedule + "/discipline";
    public const string ScheduleSubjectPosition = Schedule + "/subject-position";
    public const string ScheduleSubjectType = Schedule + "/subject-type";
    public const string ScheduleWeekType = Schedule + "/week-type";

    /*
     * Журнал.
     */

    public const string Journal = "journal";
    public const string JournalAbsence = Journal + "/absence";
    public const string JournalSession = Journal + "/session";
    public const string JournalTask = Journal + "/task";
    public const string JournalMarkType = Journal + "/mark-type";
}
namespace Fantasy.Backend.Helpers;

public static class DateTimeHelper
{
    /// <summary>
    /// Obtiene la fecha y hora actual en UTC.
    /// </summary>
    /// <returns>La fecha y hora actual en UTC.</returns>
    public static DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }
}
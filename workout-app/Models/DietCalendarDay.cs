namespace workout_app.Models;

// One cell of the diet calendar
public class DietCalendarDay
{
    public DateTime Date { get; set; }

    // 0 = Monday ... 6 = Sunday
    public int Column { get; set; }

    // False for leading/trailing days of the month grid; those cells stay empty
    public bool IsInPeriod { get; set; }

    // Average rating of the day, 0 = no entries
    public double AverageRating { get; set; }

    public bool IsToday => Date == DateTime.Today;

    public string DayText => Date.Day.ToString();
}

public class DietCalendarWeek
{
    public List<DietCalendarDay> Days { get; set; } = [];
}

public record WeekdayHeader(string Name, int Column);

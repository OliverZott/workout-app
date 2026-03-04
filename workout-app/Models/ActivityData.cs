namespace workout_app.Models;

public class ActivityData : BaseEntity
{
    public string Description { get; set; } = "";
    public ActivityType Type { get; set; }
    public double Distance { get; set; }
    public double Altitude { get; set; }
}


public enum ActivityType
{
    Running,
    Mountainabike,
    Swimming,
    Hiking,
    Skiing,
    BackcountrySkiing,
    WeightLifting,
    Other
}
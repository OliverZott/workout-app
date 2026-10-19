namespace workout_app.Models;

public class ActivityData : BaseEntity
{
    public string Description { get; set; } = "";
    public ActivityType Type { get; set; }
    public int Duration { get; set; }
    public double Distance { get; set; }
    public double Altitude { get; set; }

    // For list display
    public string DisplayDetails => string.Join(" · ", new[]
    {
        Duration >= 60
            ? $"{Duration / 60}{AppResources.unit_h} {Duration % 60}{AppResources.unit_min}"
            : $"{Duration} {AppResources.unit_min}",
        Distance > 0 ? $"{Math.Round(Distance, 1)} {AppResources.unit_km}" : null,
        Altitude > 0 ? $"{Math.Round(Altitude)} {AppResources.unit_hm}" : null
    }.Where(s => s is not null));

    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);
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
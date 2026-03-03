namespace workout_app.Models;

public class WeightData : BaseEntity
{
    public DateTime Timestamp { get; set; }
    public double Weight { get; set; }

    // For chart X-axis display
    public string DisplayDate => Timestamp.ToString("dd.MM");
}

namespace workout_app.Models;

public class BloodPressureData : BaseEntity
{
    public int Systolic { get; set; }
    public int Diastolic { get; set; }
    public int HeartRate { get; set; }
}

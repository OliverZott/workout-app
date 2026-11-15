namespace workout_app.Models;

// One row of the medication plan. The slot values are the number of units taken at that time of day.
// Timestamp (from BaseEntity) is the creation time and only used to keep the plan in insertion order.
public class MedicationData : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public int Fasting { get; set; }
    public int Morning { get; set; }
    public int Noon { get; set; }
    public int Evening { get; set; }
    public int Night { get; set; }
}

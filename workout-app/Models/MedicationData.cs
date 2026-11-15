using CommunityToolkit.Mvvm.ComponentModel;

namespace workout_app.Models;

// One row of the medication plan. The slot values are the number of units taken at that time of day.
// Timestamp (from BaseEntity) is the creation time and only used to keep the plan in insertion order.
[INotifyPropertyChanged]
public partial class MedicationData : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public int Fasting { get; set; }
    public int Morning { get; set; }
    public int Noon { get; set; }
    public int Evening { get; set; }
    public int Night { get; set; }

    // Inactive medications (e.g. only taken when needed) are greyed out and get no reminders.
    // Stored inverted so the column added to existing databases defaults to "active".
    [ObservableProperty]
    private bool isInactive;
}

using SQLite;

namespace workout_app.Models;

public class BaseEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    // For chart X-axis display
    public string DisplayDate => Timestamp.ToString("dd.MM");
}

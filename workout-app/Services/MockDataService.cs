namespace workout_app.Services;

public class MockDataService
{
    private IList<string> activityTypes = ["Walking", "Jogging", "Skiing", "Mountainbike", "Swimming", "Yoga"];
    private IList<WeightData> weightData = [];


    public MockDataService()
    {
        weightData = GenerateWeightData();
    }


    private IList<WeightData> GenerateWeightData()
    {
        var random = new Random();
        var startDate = DateTime.Today.AddDays(-100);

        return Enumerable.Range(0, 100)
            .Select(i => new WeightData
            {
                Timestamp = startDate.AddDays(i),
                Weight = random.NextDouble() * 4 + 78 // Random weight between 78 and 82 kg
            })
            .Where(d => d.Timestamp <= DateTime.Today) // Don't show future dates
            .OrderBy(d => d.Timestamp)
            .ToList();
    }

    public IList<WeightData> GetWeightData(DateTime startDate, DateTime endDate)
    {
        return [.. weightData.Where(d => d.Timestamp.Date > startDate.Date && d.Timestamp.Date <= endDate.Date)];
    }

    public WeightData? GetLastWeightDataEntry()
    {
        return weightData.LastOrDefault();
    }

    public void AddWeightDataEntry(WeightData entry)
    {
        weightData.Add(entry);
    }

    public IList<string> GetActivityTypes()
    {
        return activityTypes;
    }

}

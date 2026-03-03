using SQLite;

namespace workout_app.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db3");

        _db = new SQLiteAsyncConnection(dbPath);

        _db.CreateTableAsync<WeightData>().Wait();
    }

    // Weight
    public Task<int> AddWeightAsync(WeightData entry) =>
        _db.InsertAsync(entry);

    public Task<List<WeightData>> GetWeightsAsync() =>
        _db.Table<WeightData>().OrderByDescending(x => x.Timestamp).ToListAsync();

    // Seed data
    public async Task SeedWeightDataAsync()
    {
        var count = await _db.Table<WeightData>().CountAsync();
        if (count > 0)
            return; // schon Daten vorhanden → nicht erneut seeden

        await _db.InsertAllAsync(DatabaseSeed.Items);
    }
}

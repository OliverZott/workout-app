using SQLite;

namespace workout_app.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db3");
        _db = new SQLiteAsyncConnection(_dbPath);
    }

    public Task<int> AddWeightAsync(WeightData entry) =>
        _db.InsertAsync(entry);

    public Task<List<WeightData>> GetWeightsAsync() =>
        _db.Table<WeightData>().OrderBy(x => x.Timestamp).ToListAsync();

    public Task<int> AddCardioAsync(BloodPressureData entry) =>
    _db.InsertAsync(entry);

    public Task<List<BloodPressureData>> GetCardioAsync() =>
        _db.Table<BloodPressureData>().OrderBy(x => x.Timestamp).ToListAsync();

    public async Task InitializeAsync()
    {
        await _db.CreateTableAsync<WeightData>();
        await _db.CreateTableAsync<BloodPressureData>();

        await SeedWeightDataAsync();
        await SeedBloodPressureDataAsync();
    }

    public async Task SeedWeightDataAsync()
    {
#if DEBUG
        await _db.DeleteAllAsync<WeightData>();
        await _db.InsertAllAsync(DatabaseSeed.WeightItems);
#else
        var count = await _db.Table<WeightData>().CountAsync();
        if (count == 0)
            await _db.InsertAllAsync(DatabaseSeed.WeightItems);
#endif
    }

    public async Task SeedBloodPressureDataAsync()
    {
#if DEBUG
        await _db.DeleteAllAsync<BloodPressureData>();
        await _db.InsertAllAsync(DatabaseSeed.CardioItems);
#else
        var count = await _db.Table<BloodPressureData>().CountAsync();
        if (count == 0)
            await _db.InsertAllAsync(DatabaseSeed.CardioItems);
#endif
    }
}
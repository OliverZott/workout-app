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

    public Task<List<WeightData>> GetWeightsAsync(DateTime from, DateTime to) =>
        _db.Table<WeightData>()
            .Where(x => x.Timestamp >= from && x.Timestamp < to)
            .OrderBy(x => x.Timestamp)
            .ToListAsync();

    public Task<WeightData> GetLastWeightDataEntry() =>
        _db.Table<WeightData>().OrderByDescending(x => x.Timestamp).FirstOrDefaultAsync();

    public Task<int> AddCardioAsync(BloodPressureData entry) =>
    _db.InsertAsync(entry);

    public Task<List<BloodPressureData>> GetCardioAsync(DateTime from, DateTime to) =>
        _db.Table<BloodPressureData>()
            .Where(x => x.Timestamp >= from && x.Timestamp < to)
            .OrderBy(x => x.Timestamp)
            .ToListAsync();

    public Task<int> AddActivityAsync(ActivityData entry) =>
        _db.InsertAsync(entry);

    public Task<List<ActivityData>> GetActivitiesAsync(DateTime from, DateTime to) =>
        _db.Table<ActivityData>()
            .Where(x => x.Timestamp >= from && x.Timestamp < to)
            .OrderBy(x => x.Timestamp)
            .ToListAsync();

    public async Task InitializeAsync()
    {
        await _db.CreateTableAsync<WeightData>();
        await _db.CreateTableAsync<BloodPressureData>();
        await _db.CreateTableAsync<ActivityData>();

        await SeedWeightDataAsync();
        await SeedBloodPressureDataAsync();
        await SeedActivityDataAsync();
    }

    private async Task SeedWeightDataAsync()
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

    private async Task SeedBloodPressureDataAsync()
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

    private async Task SeedActivityDataAsync()
    {
#if DEBUG
        await _db.DeleteAllAsync<ActivityData>();
        await _db.InsertAllAsync(DatabaseSeed.ActivityItems);
#else
        var count = await _db.Table<ActivityData>().CountAsync();
        if (count == 0)
            await _db.InsertAllAsync(DatabaseSeed.ActivityItems);
#endif
    }
}
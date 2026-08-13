using SQLite;

namespace workout_app.Services;

public class DatabaseService
{
    private const string DatabaseFileName = "veolsaurus.db3";

    private SQLiteAsyncConnection? _db;
    private readonly SemaphoreSlim _dbFileGate = new(1, 1);
    private readonly string _dbPath;

    public DatabaseService()
    {
#if ANDROID
        _dbPath = Path.Combine(
            Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments)!.AbsolutePath,
            DatabaseFileName);
#else
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
#endif
    }

    private SQLiteAsyncConnection Db => _db ??= new SQLiteAsyncConnection(_dbPath);

    public Task<int> AddWeightAsync(WeightData entry) =>
        Db.InsertAsync(entry);

    public Task<List<WeightData>> GetWeightsAsync(DateTime from, DateTime to) =>
        Db.Table<WeightData>()
            .Where(x => x.Timestamp >= from && x.Timestamp < to)
            .OrderBy(x => x.Timestamp)
            .ToListAsync();

    public Task<WeightData> GetLastWeightDataEntry() =>
        Db.Table<WeightData>().OrderByDescending(x => x.Timestamp).FirstOrDefaultAsync();

    public Task<int> AddCardioAsync(BloodPressureData entry) =>
        Db.InsertAsync(entry);

    public Task<List<BloodPressureData>> GetCardioAsync(DateTime from, DateTime to) =>
        Db.Table<BloodPressureData>()
            .Where(x => x.Timestamp >= from && x.Timestamp < to)
            .OrderBy(x => x.Timestamp)
            .ToListAsync();

    public Task<int> AddActivityAsync(ActivityData entry) =>
        Db.InsertAsync(entry);

    public Task<List<ActivityData>> GetActivitiesAsync(DateTime from, DateTime to) =>
        Db.Table<ActivityData>()
            .Where(x => x.Timestamp >= from && x.Timestamp < to)
            .OrderBy(x => x.Timestamp)
            .ToListAsync();

    // Creates tables if missing and seeds initial data into empty tables.
    public async Task InitializeAsync()
    {
        await _dbFileGate.WaitAsync();
        try
        {
            await EnsureSchemaAsync();
            await SeedMissingDataAsync();
        }
        finally
        {
            _dbFileGate.Release();
        }
    }

    // Creates tables if they don't exist yet (safe to call repeatedly).
    private async Task EnsureSchemaAsync()
    {
        await Db.CreateTableAsync<WeightData>();
        await Db.CreateTableAsync<BloodPressureData>();
        await Db.CreateTableAsync<ActivityData>();
    }

    // Inserts seed data only into tables that are still empty.
    private async Task SeedMissingDataAsync()
    {
        var weightCount = await Db.Table<WeightData>().CountAsync();
        if (weightCount == 0)
            await Db.InsertAllAsync(DatabaseSeed.WeightItems);

        var bloodPressureCount = await Db.Table<BloodPressureData>().CountAsync();
        if (bloodPressureCount == 0)
            await Db.InsertAllAsync(DatabaseSeed.CardioItems);

        var activityCount = await Db.Table<ActivityData>().CountAsync();
        if (activityCount == 0)
            await Db.InsertAllAsync(DatabaseSeed.ActivityItems);
    }

    // Copies the DB file to the cache directory and returns the backup path for sharing.
    public async Task<string> CreateDatabaseBackupAsync()
    {
        await _dbFileGate.WaitAsync();
        try
        {
            if (!File.Exists(_dbPath))
                throw new FileNotFoundException("Database file not found.", _dbPath);

            await CloseConnectionInternalAsync();

            var backupPath = Path.Combine(
                FileSystem.CacheDirectory,
                $"veolsaurus-backup-{DateTime.Now:yyyyMMdd-HHmmss}.db3");

            File.Copy(_dbPath, backupPath, true);
            _db = new SQLiteAsyncConnection(_dbPath);

            return backupPath;
        }
        finally
        {
            _dbFileGate.Release();
        }
    }

    // Replaces the current DB with the file at sourceFilePath (used by Settings import).
    public async Task ReplaceDatabaseFromFileAsync(string sourceFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceFilePath);

        if (!File.Exists(sourceFilePath))
            throw new FileNotFoundException("Selected backup file was not found.", sourceFilePath);

        await _dbFileGate.WaitAsync();
        try
        {
            await using var sourceStream = File.OpenRead(sourceFilePath);
            await ReplaceDatabaseFromStreamAsync(sourceStream);
            await EnsureSchemaAsync();
        }
        finally
        {
            _dbFileGate.Release();
        }
    }

    // Closes the active connection, overwrites the DB file with the stream content, then reopens.
    private async Task ReplaceDatabaseFromStreamAsync(Stream sourceStream)
    {
        await CloseConnectionInternalAsync();

        Directory.CreateDirectory(Path.GetDirectoryName(_dbPath)!);
        await using var targetStream = File.Create(_dbPath);
        if (sourceStream.CanSeek)
            sourceStream.Position = 0;

        await sourceStream.CopyToAsync(targetStream);

        _db = new SQLiteAsyncConnection(_dbPath);
    }

    // Closes and nulls the connection so the DB file can be safely copied or replaced.
    private async Task CloseConnectionInternalAsync()
    {
        if (_db is null)
            return;

        await _db.CloseAsync();
        _db = null;
    }
}
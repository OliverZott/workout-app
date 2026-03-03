namespace workout_app.Services;

public static class DatabaseSeed
{
    public static readonly List<WeightData> Items = new()
    {
        new WeightData { Timestamp = new DateTime(2025,12,16), Weight = 88.6 },
        new WeightData { Timestamp = new DateTime(2025,12,22), Weight = 86.0 },

        new WeightData { Timestamp = new DateTime(2026,01,16), Weight = 86.4 },
        new WeightData { Timestamp = new DateTime(2026,01,17), Weight = 86.3 },
        new WeightData { Timestamp = new DateTime(2026,01,19), Weight = 86.7 },
        new WeightData { Timestamp = new DateTime(2026,01,21), Weight = 87.2 },
        new WeightData { Timestamp = new DateTime(2026,01,23), Weight = 88.1 },
        new WeightData { Timestamp = new DateTime(2026,01,25), Weight = 88.2 },
        new WeightData { Timestamp = new DateTime(2026,01,26), Weight = 88.1 },
        new WeightData { Timestamp = new DateTime(2026,01,31), Weight = 88.8 },

        new WeightData { Timestamp = new DateTime(2026,02,03), Weight = 88.6 },
        new WeightData { Timestamp = new DateTime(2026,02,04), Weight = 87.9 },
        new WeightData { Timestamp = new DateTime(2026,02,05), Weight = 87.8 },
        new WeightData { Timestamp = new DateTime(2026,02,10), Weight = 90.7 },
        new WeightData { Timestamp = new DateTime(2026,02,11), Weight = 88.8 },
        new WeightData { Timestamp = new DateTime(2026,02,12), Weight = 88.6 },
        new WeightData { Timestamp = new DateTime(2026,02,13), Weight = 89.3 },
        new WeightData { Timestamp = new DateTime(2026,02,15), Weight = 86.2 },
        new WeightData { Timestamp = new DateTime(2026,02,16), Weight = 86.1 },
        new WeightData { Timestamp = new DateTime(2026,02,18), Weight = 85.6 },
        new WeightData { Timestamp = new DateTime(2026,02,20), Weight = 85.6 },
        new WeightData { Timestamp = new DateTime(2026,02,22), Weight = 85.3 },
        new WeightData { Timestamp = new DateTime(2026,02,23), Weight = 86.6 },
        new WeightData { Timestamp = new DateTime(2026,02,24), Weight = 86.3 },
        new WeightData { Timestamp = new DateTime(2026,02,25), Weight = 86.4 },
        new WeightData { Timestamp = new DateTime(2026,02,26), Weight = 85.6 },
        new WeightData { Timestamp = new DateTime(2026,02,27), Weight = 87.4 },

        new WeightData { Timestamp = new DateTime(2026,03,02), Weight = 86.4 },
    };

}

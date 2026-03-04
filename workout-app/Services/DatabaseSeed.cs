namespace workout_app.Services;

public static class DatabaseSeed
{
    public static readonly List<WeightData> WeightItems =
    [
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

        new WeightData { Timestamp = new DateTime(2026,03,02), Weight = 86.6 },
        new WeightData { Timestamp = new DateTime(2026,03,03), Weight = 86.5 },
    ];
    public static readonly List<BloodPressureData> CardioItems =
    [
        new BloodPressureData { Timestamp = new DateTime(2025,12,16), Systolic = 177, Diastolic = 89, HeartRate = 75 },

        new BloodPressureData { Timestamp = new DateTime(2026,01,16), Systolic = 138, Diastolic = 81, HeartRate = 75 },
        new BloodPressureData { Timestamp = new DateTime(2026,01,17), Systolic = 145, Diastolic = 85, HeartRate = 72 },
        new BloodPressureData { Timestamp = new DateTime(2026,01,18), Systolic = 145, Diastolic = 89, HeartRate = 65 },
        new BloodPressureData { Timestamp = new DateTime(2026,01,19), Systolic = 144, Diastolic = 83, HeartRate = 87 },
        new BloodPressureData { Timestamp = new DateTime(2026,01,21), Systolic = 153, Diastolic = 86, HeartRate = 68 },
        new BloodPressureData { Timestamp = new DateTime(2026,01,23), Systolic = 147, Diastolic = 78, HeartRate = 76 },
        new BloodPressureData { Timestamp = new DateTime(2026,01,24), Systolic = 156, Diastolic = 89, HeartRate = 78 },
        new BloodPressureData { Timestamp = new DateTime(2026,01,25), Systolic = 147, Diastolic = 79, HeartRate = 82 },
        new BloodPressureData { Timestamp = new DateTime(2026,01,26), Systolic = 150, Diastolic = 85, HeartRate = 68 },

        new BloodPressureData { Timestamp = new DateTime(2026,02,15), Systolic = 145, Diastolic = 87, HeartRate = 61 },
        new BloodPressureData { Timestamp = new DateTime(2026,02,16), Systolic = 130, Diastolic = 81, HeartRate = 72 },
        new BloodPressureData { Timestamp = new DateTime(2026,02,18), Systolic = 146, Diastolic = 80, HeartRate = 71 },
        new BloodPressureData { Timestamp = new DateTime(2026,02,19), Systolic = 152, Diastolic = 69, HeartRate = 67 },
        new BloodPressureData { Timestamp = new DateTime(2026,02,22), Systolic = 138, Diastolic = 81, HeartRate = 69 },
        new BloodPressureData { Timestamp = new DateTime(2026,02,23), Systolic = 140, Diastolic = 73, HeartRate = 79 },
        new BloodPressureData { Timestamp = new DateTime(2026,02,25), Systolic = 136, Diastolic = 73, HeartRate = 73 },
        new BloodPressureData { Timestamp = new DateTime(2026,02,28), Systolic = 138, Diastolic = 77, HeartRate = 58 },

        new BloodPressureData { Timestamp = new DateTime(2026,03,02), Systolic = 134, Diastolic = 76, HeartRate = 64 },
        new BloodPressureData { Timestamp = new DateTime(2026,03,03), Systolic = 137, Diastolic = 82, HeartRate = 65 },
    ];


}

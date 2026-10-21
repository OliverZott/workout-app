namespace workout_app.Models;

public class DietData : BaseEntity
{
    public const int MinRating = 1;
    public const int MaxRating = 5;

    public MealType Meal { get; set; }
    public string Description { get; set; } = "";

    // 1 = healthy ... 5 = unhealthy
    public int Rating { get; set; }

    // For list display
    public string MealDisplay => GetMealName(Meal);

    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

    public static string GetMealName(MealType meal) => meal switch
    {
        MealType.Breakfast => AppResources.meal_breakfast,
        MealType.Lunch => AppResources.meal_lunch,
        MealType.Dinner => AppResources.meal_dinner,
        _ => AppResources.meal_snack
    };
}


public enum MealType
{
    Breakfast,
    Snack,
    Lunch,
    Dinner
}

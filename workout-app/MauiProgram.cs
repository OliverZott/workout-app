using CommunityToolkit.Maui;
using Syncfusion.Maui.Toolkit.Hosting;
using workout_app.PageModels;
using workout_app.Pages;
using Microsoft.Extensions.Logging;


#if ANDROID
using Android.Content.Res;
using Microsoft.Maui.Handlers;
#endif


namespace workout_app;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


#if ANDROID
        // Remove underline from all Pickers on Android (adapted from Entry solution in [SO answer](https://stackoverflow.com/a/79552152/1323381))
        PickerHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
        });
#endif

#if ANDROID
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
            handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
        });
#endif



        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainPageModel>();

        builder.Services.AddTransientWithShellRoute<WeightChartPage, WeightChartPageModel>(nameof(WeightChartPage));
        builder.Services.AddTransientWithShellRoute<WeightInsertPage, WeightInsertPageModel>(nameof(WeightInsertPage));

        // Components, Services, Utils etc. can be registered here as needed
        builder.Services.AddTransient<DateTimePickerModel>();
        builder.Services.AddSingleton<MockDataService>();
        builder.Services.AddSingleton<RadioButtonRangeToColorConverter>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

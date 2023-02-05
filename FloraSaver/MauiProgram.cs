using CommunityToolkit.Maui;
using FloraSaver.Services;
using FloraSaver.ViewModels;
using System.Collections.ObjectModel;
using Plugin.LocalNotification;

namespace FloraSaver;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("CALIFR.TTF", "CaliforniaFR");
                fonts.AddFont("PRISMA.TTF", "Prisma");
            });

#if ANDROID
            builder.UseLocalNotification();
            
#elif IOS
        builder.UseLocalNotification();
#endif

        string dbPath = FileAccessHelper.GetLocalFilePath("plant.db3");
        builder.Services.AddSingleton<TableViewModel>();
        builder.Services.AddSingleton<PlantService>(s => ActivatorUtilities.CreateInstance<PlantService>(s, dbPath));
        builder.Services.AddSingleton<PickerService>();
        builder.Services.AddSingleton<NotificationService>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<TablePage>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddTransient<PlantDetailsViewModel>();
        builder.Services.AddTransient<PlantDetailsPage>();
        return builder.Build();
    }
}

public static class ObservableCollectionExtensionMethods
{
    public static void Replace<T>(this ObservableCollection<T> current, ObservableCollection<T> @new)
    {
        current.Clear();
        foreach (var item in @new)
        {
            current.Add(item);
        }
    }
}

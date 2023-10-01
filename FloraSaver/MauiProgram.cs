using CommunityToolkit.Maui;
using FloraSaver.Services;
using FloraSaver.ViewModels;
using System.Collections.ObjectModel;
using Plugin.LocalNotification;
using CommunityToolkit.Maui.Storage;

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
        builder.Services.AddSingleton<Base64ImageConverterService>();
        builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
        builder.Services.AddSingleton<DBImportExportService>();
        builder.Services.AddSingleton<HandlingViewModel>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>(s => ActivatorUtilities.CreateInstance<DatabaseService>(s, dbPath));
        builder.Services.AddSingleton<PickerService>();
        builder.Services.AddSingleton<IPlantNotificationService, PlantNotificationService>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<TablePage>();
        builder.Services.AddSingleton<HandlingPage>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<PlantDetailsSetupViewModel>();
        builder.Services.AddTransient<PlantDetailsSetupPage>();
        builder.Services.AddTransient<PlantDetailsViewModel>();
        builder.Services.AddTransient<PlantDetailsPage>();
        builder.Services.AddTransient<ClipetOverlayViewModel>();
        builder.Services.AddTransient<BackupRestoreViewModel>();
        builder.Services.AddTransient<ClipetOverlayPage>();
        builder.Services.AddTransient<DatabaseExportPage>();
        
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

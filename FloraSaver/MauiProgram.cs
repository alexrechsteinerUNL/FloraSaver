using CommunityToolkit.Maui;
using FloraSaver.Services;
using FloraSaver.ViewModels;
using System.Collections.ObjectModel;

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
                fonts.AddFont("CALIFR.ttf", "CaliforniaFR");
                fonts.AddFont("PRISMA.ttf", "Prisma");
            });

        string dbPath = FileAccessHelper.GetLocalFilePath("plant.db3");
        builder.Services.AddSingleton<TableViewModel>();
        builder.Services.AddSingleton<PlantService>(s => ActivatorUtilities.CreateInstance<PlantService>(s, dbPath));
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<TablePage>();
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

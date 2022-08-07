using FloraSaver.Services;
using FloraSaver.ViewModels;

namespace FloraSaver;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("CALIFR.ttf", "CaliforniaFR");
                fonts.AddFont("PRISMA.ttf", "Prisma");
            });

		string dbPath = FileAccessHelper.GetLocalFilePath("plant.db3");
        builder.Services.AddSingleton<PlantsViewModel>();
        builder.Services.AddSingleton<PlantService>(s => ActivatorUtilities.CreateInstance<PlantService>(s, dbPath));
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<TablePage>();
        return builder.Build();
	}
}

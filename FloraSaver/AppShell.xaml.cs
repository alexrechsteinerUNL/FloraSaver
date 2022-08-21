namespace FloraSaver;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(PlantDetailsPage), typeof(PlantDetailsPage));
    }
}

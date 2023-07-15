﻿namespace FloraSaver;

public partial class AppShell : Shell
{
	public Dictionary<string, Type> Routes { get; set; } = new Dictionary<string, Type>();
	public AppShell()
	{
		InitializeComponent();
		Routes.Add(nameof(PlantDetailsPage), typeof(PlantDetailsPage));
        Routes.Add(nameof(ClipetOverlayPage), typeof(ClipetOverlayPage));

        foreach (var route in Routes)
		{
            Routing.RegisterRoute(route.Key, route.Value);
        }
        
    }
}

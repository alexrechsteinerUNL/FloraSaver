namespace FloraSaver;

public partial class MainPage : ContentPage
{
	IServiceNotification NotificationServices;
	public MainPage(IServiceNotification _NotificationServices)
	{
		InitializeComponent();
		NotificationServices = _NotificationServices;


    }

    private void OnServiceStartClicked(object sender, EventArgs e)
    {
        NotificationServices.Start();
    }
    //method to stop manually foreground service
    private void Button_Clicked(object sender, EventArgs e)
    {
        NotificationServices.Stop();
    }
}


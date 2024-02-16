using FloraSaver.Services;
using Microsoft.VisualStudio.Threading;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace FloraSaver;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;
        MainPage = new AppShell();
    }

    private void OnNotificationActionTapped(NotificationActionEventArgs e)
    {
        if (e.IsDismissed)
        {
            // your code goes here
            return;
        }
        if (e.IsTapped)
        {
            // your code goes here
            Shell.Current.GoToAsync($"///{nameof(TablePage)}", true, new Dictionary<string, object>
            {
                {"ShouldGetNewData", true },
                {"ShouldGetNewGroupData", true },
                {"ScrollToPlant", e.Request.ReturningData }
            });
            return;
        }
        // if Notification Action are setup
        //switch (e.ActionId)
    }
}
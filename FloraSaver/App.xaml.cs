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
        Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
        MainPage = new AppShell();

    }



    private void OnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
    {
    #if ANDROID
        AndroidX.AppCompat.App.AppCompatDelegate.DefaultNightMode = Current.UserAppTheme switch
        {
            AppTheme.Light => AndroidX.AppCompat.App.AppCompatDelegate.ModeNightNo,
            AppTheme.Dark => AndroidX.AppCompat.App.AppCompatDelegate.ModeNightYes,
            _ => AndroidX.AppCompat.App.AppCompatDelegate.ModeNightFollowSystem
        };

    #elif IOS
            Platform.GetCurrentUIViewController().OverrideUserInterfaceStyle = Current.UserAppTheme switch
            {
                AppTheme.Light => UIKit.UIUserInterfaceStyle.Light,
                AppTheme.Dark => UIKit.UIUserInterfaceStyle.Dark,
                _ => UIKit.UIUserInterfaceStyle.Unspecified
            };
    #endif


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
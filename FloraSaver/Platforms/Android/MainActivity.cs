using Android.App;
using Android.Content.PM;

namespace FloraSaver;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    //set an activity on main application to get the reference on the service (check this stackoverflow page: https://stackoverflow.com/questions/71259615/how-to-create-a-background-service-in-net-maui)
    public static MainActivity ActivityCurrent { get; set; }
    public MainActivity()
    {
        ActivityCurrent = this;
    }
}
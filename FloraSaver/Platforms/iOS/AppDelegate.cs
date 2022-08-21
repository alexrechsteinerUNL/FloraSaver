using Foundation;
using SQLitePCL;

namespace FloraSaver;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        raw.SetProvider(new SQLite3Provider_dynamic_cdecl());
        return MauiProgram.CreateMauiApp();
    }
}

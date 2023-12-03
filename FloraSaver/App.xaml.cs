using FloraSaver.Services;
using Microsoft.VisualStudio.Threading;

namespace FloraSaver;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}
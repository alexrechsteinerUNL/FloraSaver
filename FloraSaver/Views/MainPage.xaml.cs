using FloraSaver.ViewModels;
using FloraSaver.Utilities;

namespace FloraSaver;

public partial class MainPage : ContentPage, IAndroidBackButtonHandlerUtility
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        ((MainViewModel)(this.BindingContext)).ReconfigureValuesForScreenSize(width, height);
        
        if (height < 600)
        {
            _FullMode.IsVisible = false;
            _CompactMode.IsVisible = true;
            
        } else
        {
            _FullMode.IsVisible = true;
            _CompactMode.IsVisible = false;
        }
    }

    public async Task<bool> HandleBackButtonAsync()
    {
        return await ((BaseViewModel)(this.BindingContext)).BackButtonWarnLeavingApplicationAsync() ? false : true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}
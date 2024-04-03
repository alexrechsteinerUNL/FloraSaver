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

    private async void TreatPressedAsync(object sender, EventArgs e)
    {
        if (_CompactMode.IsVisible)
        {
            var currentHeight = (double)compactTreat.Y;
            if (currentHeight == 0)
            {
                currentHeight = (double)compactClipet.Y;
            }
            compactTreat.IsVisible = true;
            await compactTreat.TranslateTo(150, currentHeight, 0);
            var startGiveTreat = compactTreat.TranslateTo(-50, -(currentHeight / 20), 500, Easing.CubicInOut);
            compactClipet.Source = "clipet_flowers_standing_color_eat.png";
            await startGiveTreat;
            compactTreat.IsVisible = false;
            compactClipet.Source = "clipet_flowers_standing_color.png";
        }
        else
        {
            var currentHeight =  (double)fullTreat.Y;
            if (currentHeight == 0)
            {
                currentHeight = (double)fullClipet.Y;
            }
            fullTreat.IsVisible = true;
            await fullTreat.TranslateTo(300, currentHeight,0);

            var startGiveTreat = fullTreat.TranslateTo(-45, -(currentHeight / 20), 500, Easing.CubicInOut);
            fullClipet.Source = "clipet_flowers_standing_color_eat.png";
            await startGiveTreat;
            fullTreat.IsVisible = false;
            fullClipet.Source = "clipet_flowers_standing_color.png";


        }
    }
}
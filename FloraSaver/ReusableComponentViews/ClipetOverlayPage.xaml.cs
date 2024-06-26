using CommunityToolkit.Mvvm.ComponentModel;
using FloraSaver.Models;
using FloraSaver.ViewModels;
using System.ComponentModel;

namespace FloraSaver;

public partial class ClipetOverlayPage : ContentPage
{
    public static readonly BindableProperty isClipetEnabledProperty = BindableProperty.Create(nameof(isClipetEnabled), typeof(bool?), typeof(ClipetOverlayPage), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ClipetOverlayPage)bindable;

        control.clipet.IsVisible = (bool)newValue;
    });

    public static readonly BindableProperty ClipetDataProperty = BindableProperty.Create(nameof(ClipetData), typeof(List<ClipetSpeechBubble>), typeof(ClipetOverlayPage), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ClipetOverlayPage)bindable;

        List<ClipetSpeechBubble> bloop = (List<ClipetSpeechBubble>)newValue;
    });

    public static readonly BindableProperty isBlurEnabledProperty = BindableProperty.Create(nameof(isBlurEnabled), typeof(bool?), typeof(ClipetOverlayPage), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ClipetOverlayPage)bindable;

        control.blur.IsVisible = (bool)newValue;
    });

    public static readonly BindableProperty isDisplaySpaceEnabledProperty = BindableProperty.Create(nameof(isDisplaySpaceEnabled), typeof(bool?), typeof(ClipetOverlayPage), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ClipetOverlayPage)bindable;

        control.displaySpace.IsVisible = (bool)newValue;
    });

    public static readonly BindableProperty isSpeechSpaceEnabledProperty = BindableProperty.Create(nameof(isSpeechSpaceEnabled), typeof(bool?), typeof(ClipetOverlayPage), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ClipetOverlayPage)bindable;

        control.speechSpace.IsVisible = (bool)newValue;
    });

    public ClipetOverlayPage(ClipetOverlayViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        blur.IsVisible = true;
        var startBlur = blur.FadeTo(1, 500);
        await clipet.TranslateTo(0, 500, 10);
        clipet.IsVisible = true;
        await clipet.TranslateTo(0, 0, 500);
        await startBlur;

    }

    public bool? isClipetEnabled
    {
        get => GetValue(isClipetEnabledProperty) as bool?;
        set => SetValue(isClipetEnabledProperty, value);
    }

    public List<ClipetSpeechBubble> ClipetData
    {
        get => GetValue(ClipetDataProperty) as List<ClipetSpeechBubble>;
        set => SetValue(ClipetDataProperty, value);
    }

    public bool? isBlurEnabled
    {
        get => GetValue(isBlurEnabledProperty) as bool?;
        set => SetValue(isBlurEnabledProperty, value);
    }

    public bool? isDisplaySpaceEnabled
    {
        get => GetValue(isDisplaySpaceEnabledProperty) as bool?;
        set => SetValue(isDisplaySpaceEnabledProperty, value);
    }

    public bool? isSpeechSpaceEnabled
    {
        get => GetValue(isSpeechSpaceEnabledProperty) as bool?;
        set => SetValue(isSpeechSpaceEnabledProperty, value);
    }

    private void Frame_SizeChanged(object sender, EventArgs e)
    {
        //var fontSize = 24.0;
        //var frameWidth = Frame.Width;
        //var frameHeight = Frame.Height;
        ////My brain is shot here. Head empty. Fix this tomorrow.

        //if (frameWidth/frameHeight > 1.2)
        //{
        //    fontSize = (fontSize - 4 * (frameWidth / frameHeight));
        //}

        //if (frameWidth < 600 && frameHeight < 600)
        //{
        //    if (frameWidth < frameHeight)
        //    {
        //        fontSize = (fontSize * (frameWidth / 1400) + 6);
        //    } else
        //    {
        //        fontSize = fontSize = (fontSize * (frameHeight / 1400) + 6);
        //    }
            
        //}
        //speech.FontSize = fontSize;
    }

    private async void HiddenActionForDisappearing_HandlerChanged(object sender, EventArgs e)
    {
        var label = sender as Label;
        if (!label.IsEnabled)
        {
            await blur.FadeTo(0, 300);
            await clipet.TranslateTo(0, 500, 300);
            label.IsEnabled = true;
            await Shell.Current.GoToAsync("..", true);
        }
        
    }
}
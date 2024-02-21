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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        blur.IsVisible = true;
        blur.FadeTo(1, 2000);
        clipet.IsVisible = true;

    }

    protected override void OnDisappearing()
    {
        blur.FadeTo(0, 2000);
        base.OnDisappearing();
        
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
}
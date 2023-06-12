using CommunityToolkit.Mvvm.ComponentModel;
using FloraSaver.Models;
using FloraSaver.ViewModels;

namespace FloraSaver;

public partial class ClipetOverlay : ContentPage
{
    public static readonly BindableProperty isClipetEnabledProperty = BindableProperty.Create(nameof(isClipetEnabled), typeof(bool?), typeof(ClipetOverlay), propertyChanged:(bindable, oldValue, newValue) => 
    {
        var control = (ClipetOverlay)bindable;

        control.clipet.IsVisible = (bool)newValue;
    });

    public static readonly BindableProperty ClipetDataProperty = BindableProperty.Create(nameof(ClipetData), typeof(List<ClipetSpeechBubble>), typeof(ClipetOverlay), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ClipetOverlay)bindable;

        //control.clipet.IsVisible = (List<ClipetSpeechBubble>)newValue;
    });

    public static readonly BindableProperty isBlurEnabledProperty = BindableProperty.Create(nameof(isBlurEnabled), typeof(bool?), typeof(ClipetOverlay), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ClipetOverlay)bindable;

        control.blur.IsVisible = (bool)newValue;
    });

    public static readonly BindableProperty isDisplaySpaceEnabledProperty = BindableProperty.Create(nameof(isDisplaySpaceEnabled), typeof(bool?), typeof(ClipetOverlay), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ClipetOverlay)bindable;

        control.displaySpace.IsVisible = (bool)newValue;
    });

    public static readonly BindableProperty isSpeechSpaceEnabledProperty = BindableProperty.Create(nameof(isSpeechSpaceEnabled), typeof(bool?), typeof(ClipetOverlay), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ClipetOverlay)bindable;

        control.speechSpace.IsVisible = (bool)newValue;
    });
    public ClipetOverlayViewModel ClipetViewModel { get; private set; }
    public ClipetOverlay()
    {
        InitializeComponent();
        ClipetViewModel = new ClipetOverlayViewModel();

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
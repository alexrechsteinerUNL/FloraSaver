using FloraSaver.ViewModels;

namespace FloraSaver;

public partial class ClipetOverlay : ContentPage
{
    public static readonly BindableProperty isClipetEnabledProperty = BindableProperty.Create(nameof(isClipetEnabled), typeof(bool?), typeof(ClipetOverlay), propertyChanged:(bindable, oldValue, newValue) => 
    {
        var control = (ClipetOverlay)bindable;

        control.bloop.IsVisible = (bool)newValue;
    });

    public ClipetOverlay()
	{
        InitializeComponent();
    }

    public bool? isClipetEnabled 
    {
        get => GetValue(isClipetEnabledProperty) as bool?; 
        set => SetValue(isClipetEnabledProperty, value);
    }



}
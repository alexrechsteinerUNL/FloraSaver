using CommunityToolkit.Mvvm.ComponentModel;
using FloraSaver.Services;

namespace FloraSaver.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        public bool isBusy;

        [ObservableProperty]
        string title;

        public PlantService plantService;

        // I think the code below is equivalent to
        // [ObservableProperty]
        // string friendlyLabel;
        string friendlyLabel;
        public string FriendlyLabel
        {
            get => friendlyLabel;
            set
            {
                friendlyLabel = value;
                OnPropertyChanged();
            }
        }


        public bool IsNotBusy => !IsBusy;


    }
}
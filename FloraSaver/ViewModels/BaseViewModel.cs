using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
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

        public IDatabaseService _databaseService;

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
        
        [RelayCommand]
        public async Task UpdateNotifications(List<Plant> Plants)
        {
            await _databaseService.GetAllPlantAsync();
        }


    }
}
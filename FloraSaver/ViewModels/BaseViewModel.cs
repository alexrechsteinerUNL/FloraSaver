using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;
using FloraSaver.Utilities;

namespace FloraSaver.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        public bool isBusy;

        [ObservableProperty]
        string title;

        [ObservableProperty]
        List<ClipetSpeechBubble> clipetDialog;

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

        [RelayCommand]
        public async Task LoadClipetTextFileAsync(string filename)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(filename);
            using var reader = new StreamReader(stream);

            var contents = await reader.ReadToEndAsync();
            var formatDialogFromResourceUtility = new FormatDialogFromResourceUtility();
            ClipetDialog = formatDialogFromResourceUtility.SortTextBoxes(contents);
            OnPropertyChanged(nameof(List<ClipetSpeechBubble>));
        }


    }
}
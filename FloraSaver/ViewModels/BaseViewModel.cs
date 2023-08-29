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
        public bool isFriendlyLabelVisible = false;
        [ObservableProperty]
        string title;

        public IDatabaseService _databaseService;

        // I think the code below is equivalent to
        // [ObservableProperty]
        // string friendlyLabel;
        // also don't do this in the setter, are you crazy?
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
        public async Task<string> PickedImageToBase64Async()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images
            });

            if (result is null)
            {
                return null;
            }
            // extract below to extension method or helper class for base64
            var imageStream = await result.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream);
            //AlterPlant.ImageLocation = Convert.ToBase64String(memoryStream.ToArray());
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public ImageSource Base64ToImage(string base64String)
        {
            var bytesOfImage = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(bytesOfImage);
            return ImageSource.FromStream(() => memoryStream);
        }

        public async Task FriendlyLabelToastAsync()
        {
            IsFriendlyLabelVisible = true;
            await Task.Delay(5000);
            IsFriendlyLabelVisible = false;
        }
        public bool IsNotBusy => !IsBusy;
        
        [RelayCommand]
        public async Task UpdateNotifications(List<Plant> Plants)
        {
            await _databaseService.GetAllPlantAsync();
        }

        [RelayCommand]
        public async Task TalkToClipetAsync(string filename)
        {
            var clipetDialog = await LoadClipetTextFileAsync(filename);
            await GoToClipetOverlayAsync(clipetDialog);
        }

        [RelayCommand]
        public async Task<List<ClipetSpeechBubble>> LoadClipetTextFileAsync(string filename)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(filename);
            using var reader = new StreamReader(stream);

            var contents = await reader.ReadToEndAsync();
            var formatDialogFromResourceUtility = new FormatDialogFromResourceUtility();
            return formatDialogFromResourceUtility.SortTextBoxes(contents);
        }

        [RelayCommand]
        async Task GoToClipetOverlayAsync(List<ClipetSpeechBubble> speechBubbles)
        {
            if (speechBubbles == null)
            {
                await Shell.Current.GoToAsync(nameof(ClipetOverlayPage), true, new Dictionary<string, object>());
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(ClipetOverlayPage), true, new Dictionary<string, object> { { "ClipetSpeechBubbles", speechBubbles } });
            }
            return;
        }

    }
}
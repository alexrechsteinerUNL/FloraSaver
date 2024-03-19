using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;
using FloraSaver.Utilities;
using System.Collections.ObjectModel;

namespace FloraSaver.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        public bool shouldUpdateGroupFromDB = false;
        public bool shouldUpdatePlantsFromDB = false;

        public static List<AutoFillPlant> PlantSuggestions { get; set; } = new();
        public ObservableCollection<Plant> DataPlants { get; set; } = new();
        public ObservableCollection<SearchedPlants> TopTenAutoFillPlants { get; set; } = new();

        [RelayCommand]
        protected void QueryAutofillPlantAsyncFromSearch(string searchQuery)
        {
            TopTenAutoFillPlants = new ObservableCollection<SearchedPlants>();
            var topPlants = DataPlants.Where(_ => _.GivenName.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase) || _.PlantSpecies.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase));
            foreach (var plant in topPlants)
            {
                TopTenAutoFillPlants.Add(new SearchedPlants(plant));
            }
            var topTenSuggestionPlants = PlantSuggestions.Where(_ => _.PlantSpecies.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase)).Take(10);
            foreach (var plant in topTenSuggestionPlants)
            {
                TopTenAutoFillPlants.Add(new SearchedPlants(plant));
            }
            OnPropertyChanged("TopTenAutoFillPlants");
            ShowSearchSuggestionBox();
        }

        [RelayCommand]
        protected void ShowSearchSuggestionBox()
        {
            ShowSearchSuggestionsBox = true;
        }

        [RelayCommand]
        protected void HideSearchSuggestionBox()
        {
            ShowSearchSuggestionsBox = false;
        }
        [ObservableProperty]
        private bool showSearchSuggestionsBox = false;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        public bool isBusy;

        [ObservableProperty]
        public bool isFriendlyLabelVisible = false;

        [ObservableProperty]
        private string title;

        public IDatabaseService _databaseService;

        // I think the code below is equivalent to
        // [ObservableProperty]
        // string friendlyLabel;
        // also don't do this in the setter, are you crazy?
        private string friendlyLabel;

        public string FriendlyLabel
        {
            get => friendlyLabel;
            set
            {
                friendlyLabel = value;
                OnPropertyChanged();
            }
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
        private async Task GoToClipetOverlayAsync(List<ClipetSpeechBubble> speechBubbles)
        {
            if (speechBubbles == null)
            {
                await Shell.Current.GoToAsync(nameof(ClipetOverlayPage), false, new Dictionary<string, object>());
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(ClipetOverlayPage), false, new Dictionary<string, object> { { "ClipetSpeechBubbles", speechBubbles } });
            }
            return;
        }
    }
}
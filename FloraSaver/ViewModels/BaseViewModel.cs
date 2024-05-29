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

        [ObservableProperty]
        public bool isInitialization = false;

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
            OnPropertyChanged(nameof(TopTenAutoFillPlants));
            ShowSearchSuggestionBox();
        }

        [RelayCommand]
        private async Task GoToDatabaseExportAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(DatabaseExportPage)}", true);
        }

        [RelayCommand]
        private async Task GoToDatabaseImportAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(DatabaseImportPage)}", true);
        }

        [RelayCommand]
        private async Task GoToAllClipetDialogsAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(AllClipetDialogs)}", true);
        }

        [RelayCommand]
        public async Task<bool> BackButtonWarnLeavingApplicationAsync()
        {
            return await Application.Current.MainPage.DisplayAlert("AH WAIT!", "Are you sure you want to close the app?", "Please Close", "No Don't!");
        }
        [ObservableProperty]
        public List<TemperatureInterval> temperatureIntervalsF;
        [ObservableProperty]
        public List<TemperatureInterval> temperatureIntervalsC;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TemperatureButtonText))]
        [NotifyPropertyChangedFor(nameof(IsFahrenheit))]
        public bool isCelsius = false;

        public string TemperatureButtonText => IsCelsius ? "C" : "F";
        public bool IsFahrenheit => !IsCelsius;

        [ObservableProperty]
        public bool showTutorial = true;

        public bool IsChangingCtoF = false;

        [ObservableProperty]
        TemperatureInterval temperatureIntervalPickerValueF;
        [ObservableProperty]
        TemperatureInterval temperatureIntervalPickerValueC;

        [ObservableProperty]
        public List<HumidityInterval> humidityIntervals;
        [ObservableProperty]
        public HumidityInterval humidityIntervalPickerValue;

        [RelayCommand]
        partial void OnHumidityIntervalPickerValueChanged(HumidityInterval value)
        {
            Preferences.Default.Set("humidity_level", value.HumidityLevel);
            HumidityIntervalPickerValue = value;
        }

        [RelayCommand]
        partial void OnTemperatureIntervalPickerValueFChanged(TemperatureInterval value)
        {
            if (!IsChangingCtoF && !IsInitialization)
            {
                IsChangingCtoF = true;
                if (TemperatureIntervalPickerValueC is null) { TemperatureIntervalPickerValueC = value; }
                Preferences.Default.Set("temperature_level", value.TemperatureLevel);
                TemperatureIntervalPickerValueF.TemperatureLevel = value.TemperatureLevel;
                TemperatureIntervalPickerValueC.TemperatureLevel = value.TemperatureLevel;
            }
            IsChangingCtoF = false;
        }

        [RelayCommand]
        partial void OnTemperatureIntervalPickerValueCChanged(TemperatureInterval value)
        {
            if (!IsChangingCtoF && !IsInitialization)
            {
                IsChangingCtoF = true;
                if (TemperatureIntervalPickerValueF is null) { TemperatureIntervalPickerValueF = value; }
                Preferences.Default.Set("temperature_level", value.TemperatureLevel);
                TemperatureIntervalPickerValueF.TemperatureLevel = value.TemperatureLevel;
                TemperatureIntervalPickerValueC.TemperatureLevel = value.TemperatureLevel;
            }
            IsChangingCtoF = false;
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

        [RelayCommand]
        public async Task HelpMessageAlertAsync(string message)
        {
            await Application.Current.MainPage.DisplayAlert("Page Info!", $"{message}", "Sounds Good");
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
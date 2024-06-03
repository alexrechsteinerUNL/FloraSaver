using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Plugin.LocalNotification;

namespace FloraSaver.ViewModels
{
    [QueryProperty(nameof(Plant), "Plant")]
    [QueryProperty(nameof(PlantGroup), "PlantGroup")]
    public partial class PlantDetailsViewModel : BaseViewModel, IQueryAttributable, INotifyPropertyChanged
    {
        protected double InitialWaterDaysFromNow = 0;
        protected double InitialMistDaysFromNow = 0;
        protected double InitialSunDaysFromNow = 0;

        public List<string> UnsafePlantNames { get; set; }

        [ObservableProperty]
        protected ImageSource currentPlantImage;


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EditActionsButtonText))]
        public bool isAdvancedMode = false;
        public string EditActionsButtonText => IsAdvancedMode ? "Hide Actions" : "Edit Actions";
        [RelayCommand]
        public void ShowHideAdvancedOptions() { if (IsAdvancedMode) { IsAdvancedMode = false; } else { IsAdvancedMode = true; } } 

        [ObservableProperty]
        protected bool isBeingUndone = false;

        [ObservableProperty]
        protected bool isBeingAutoAdjusted = false;

        [ObservableProperty]
        public HumidityInterval humidityIntervalPickerValueDetails;
        [ObservableProperty]
        TemperatureInterval temperatureIntervalPickerValueFDetails;
        [ObservableProperty]
        TemperatureInterval temperatureIntervalPickerValueCDetails;

        [ObservableProperty]
        public string saveText = "Add";
        [ObservableProperty]
        public string backText = "Back";

        public string NewGroupValidation(PlantGroup plantGroup)
        {
            if (!IsInitialization && !IsBeingUndone)
            {
                plantGroup.Validate(PlantGroups.Select(_ => _.GroupName).ToList());
            }
            return plantGroup.Validation.Message;
        }


        // I moved the _databaseService to the base viewmodel because just about every page was going to use it.
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GroupPickerUnsavedChangesWarning))]
        protected bool groupUndoButtonVisible = false;
        [RelayCommand]
        protected void GroupPickerChanged() { GroupUndoButtonVisible = (!IsInitialization && !IsBeingUndone) ? true : false;}
        public string GroupPickerUnsavedChangesWarning => GroupUndoButtonVisible ? "• Plant Group\n" : "";



        [RelayCommand]
        protected void GroupSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.PlantGroupName = InitialPlant.PlantGroupName;
            AlterPlant.GroupColorHexString = InitialPlant.GroupColorHexString;
            GroupPickerValue = AlterPlant.PlantGroupName != null ? PlantGroups.FirstOrDefault(_ => _.GroupName == AlterPlant.PlantGroupName) : PlantGroups.FirstOrDefault(_ => _.GroupName == "Ungrouped");
            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            GroupUndoButtonVisible = false;
        }

        

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageUnsavedChangesWarning))]
        protected bool imageUndoButtonVisible = false;
        [RelayCommand]
        protected void ImageChanged() { ImageUndoButtonVisible = (!IsInitialization && !IsBeingUndone) ? true : false;}
        public string ImageUnsavedChangesWarning => ImageUndoButtonVisible ? "• Plant Image\n" : "";
        [RelayCommand]
        protected void ImageChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.ImageLocation = InitialPlant.ImageLocation;
            AlterPlant.PlantImageSource = InitialPlant.PlantImageSource;

            if (AlterPlant.ImageLocation is not null)
            {
                SetImageSourceOfPlant();
                IsImageSelected = true;
            }
            else
            {
                IsImageSelected = false;
            }
            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            ImageUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HumidityUnsavedChangesWarning))]
        protected bool humidityUndoButtonVisible = false;
        [RelayCommand]
        protected void HumidityChanged() { HumidityUndoButtonVisible = (!IsInitialization && !IsBeingUndone && AlterPlant.HumanityIntervalInt != (InitialPlant.HumanityIntervalInt ?? Preferences.Default.Get("humidity_level", 44)) ? true : false); }
        public string HumidityUnsavedChangesWarning => HumidityUndoButtonVisible ? "• Plant Humidity\n" : "";
        [RelayCommand]
        protected void HumidityChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.HumanityIntervalInt = InitialPlant.HumanityIntervalInt ?? InitialPlant.HumanityIntervalInt ?? Preferences.Default.Get("humidity_level", 44);
            OnPropertyChanged(nameof(AlterPlant));
            HumidityIntervalPickerValueDetails = HumidityIntervals.FirstOrDefault(_ => _.HumidityLevel == AlterPlant.HumanityIntervalInt) ?? new HumidityInterval() { HumidityLevel = 30, IntervalText = "Normal Indoor Humidity" };
            OnPropertyChanged(nameof(HumidityIntervalPickerValueDetails));

            IsBeingUndone = false;
            HumidityUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TemperatureUnsavedChangesWarning))]
        protected bool temperatureUndoButtonVisible = false;
        [RelayCommand]
        protected void TemperatureChanged() { TemperatureUndoButtonVisible = (!IsInitialization && !IsBeingUndone && AlterPlant.TemperatureIntervalInt != (InitialPlant.TemperatureIntervalInt ?? Preferences.Default.Get("Temperature_level", 60)) ? true : false); }
        public string TemperatureUnsavedChangesWarning => TemperatureUndoButtonVisible ? "• Plant Temperature\n" : "";
        [RelayCommand]
        protected void TemperatureChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.TemperatureIntervalInt = InitialPlant.TemperatureIntervalInt ?? Preferences.Default.Get("Temperature_level", 60);
            OnPropertyChanged(nameof(AlterPlant));
            TemperatureIntervalPickerValueFDetails = TemperatureIntervalsF.FirstOrDefault(_ => _.TemperatureLevel == AlterPlant.TemperatureIntervalInt) ?? TemperatureIntervalsF.FirstOrDefault(_ => _.TemperatureLevel == Preferences.Default.Get("Temperature_level", 60));
            TemperatureIntervalPickerValueCDetails = TemperatureIntervalsC.FirstOrDefault(_ => _.TemperatureLevel == AlterPlant.TemperatureIntervalInt) ?? TemperatureIntervalsC.FirstOrDefault(_ => _.TemperatureLevel == Preferences.Default.Get("Temperature_level", 60));
            OnPropertyChanged(nameof(TemperatureIntervalPickerValueCDetails));

            IsBeingUndone = false;
            TemperatureUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SpeciesUnsavedChangesWarning))]
        protected bool speciesUndoButtonVisible = false;
        [RelayCommand]
        protected void SpeciesChanged() { SpeciesUndoButtonVisible = (!IsInitialization && !IsBeingUndone && AlterPlant.PlantSpecies != InitialPlant.PlantSpecies) ? true : false;}
        public string SpeciesUnsavedChangesWarning => SpeciesUndoButtonVisible ? "• Plant Type\n" : "";
        [RelayCommand]
        protected void SpeciesChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.PlantSpecies = InitialPlant.PlantSpecies;
            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            SpeciesUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GivenNameUnsavedChangesWarning))]
        protected bool givenNameUndoButtonVisible = false;
        [RelayCommand]
        protected void GivenNameChanged() { GivenNameUndoButtonVisible = (!IsInitialization && !IsBeingUndone && AlterPlant.GivenName != InitialPlant.GivenName) ? true : false; }
        public string GivenNameUnsavedChangesWarning => GivenNameUndoButtonVisible ? "• Plant Name\n" : "";
        [RelayCommand]
        protected void GivenNameChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.GivenName = InitialPlant.GivenName;
            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            GivenNameUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DateOfBirthUnsavedChangesWarning))]
        protected bool dobUndoButtonVisible = false;
        [RelayCommand]
        protected void DobChanged() { DobUndoButtonVisible = (!IsInitialization && !IsBeingUndone && AlterPlant.DateOfBirth != InitialPlant.DateOfBirth) ? true : false; }
        public string DateOfBirthUnsavedChangesWarning => DobUndoButtonVisible ? "• Date of Birth\n" : "";
        [RelayCommand]
        protected void DobChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.DateOfBirth = InitialPlant.DateOfBirth;
            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            DobUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WaterIntervalUnsavedChangesWarning))]
        protected bool waterIntervalUndoButtonVisible = false;
        [RelayCommand]
        protected void WaterIntervalChanged() {
            WaterIntervalUndoButtonVisible = (!IsInitialization && !IsBeingUndone && (AlterPlant.WaterInterval != InitialPlant.WaterInterval)) ? true : false;
            if (!IsBeingAutoAdjusted && !IsInitialization && !IsBeingUndone)
            {
                AlterPlant.BaseWaterIntervalForTempAndHum = AlterPlant.FindBase((AlterPlant.DateOfNextWatering.Date - AlterPlant.DateOfLastWatering.Date).Days);
            }
        }
        public string WaterIntervalUnsavedChangesWarning => WaterIntervalUndoButtonVisible ? "• Water Interval\n" : "";
        [RelayCommand]
        protected void WaterIntervalChangedSectionUndo()
        {
            IsBeingUndone = true;
            HumidityChangedSectionUndo();
            TemperatureChangedSectionUndo();
            AlterPlant.WaterInterval = InitialPlant.WaterInterval;
            WaterDaysFromNow = AlterPlant.WaterInterval != null ? (int)AlterPlant.WaterInterval : (AlterPlant.DateOfNextWatering.Date - AlterPlant.DateOfLastWatering.Date).Days;
            IsInitialization = true;
            WaterIntervalPickerValue = WateringInterval.FirstOrDefault(x => x.NumFromNow == InitialWaterDaysFromNow);
            WaterIntervalPickerValue ??= WateringInterval.First(x => x.NumFromNow == -1);
            IsInitialization = false;
            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            WaterIntervalUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LastWateredUnsavedChangesWarning))]
        protected bool lastWateredUndoButtonVisible = false;
        [RelayCommand]
        protected void LastWateredChanged()
        {
            LastWateredUndoButtonVisible = (!IsInitialization
                                                                            && !IsBeingUndone
                                                                            && (AlterPlant.DateOfLastWatering != InitialPlant.DateOfLastWatering
                                                                            || AlterPlant.TimeOfLastWatering != InitialPlant.TimeOfLastWatering))
                                                                            ? true : false;
        }
        public string LastWateredUnsavedChangesWarning => LastWateredUndoButtonVisible ? "• Last Watering\n" : "";
        [RelayCommand]
        protected void LastWateredChangedSectionUndo()
        {
            IsBeingUndone = true;
            HumidityChangedSectionUndo();
            TemperatureChangedSectionUndo();
            AlterPlant.DateOfLastWatering = InitialPlant.DateOfLastWatering;
            AlterPlant.TimeOfLastWatering = InitialPlant.TimeOfLastWatering;
            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            LastWateredUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NextWateringUnsavedChangesWarning))]
        protected bool nextWaterUndoButtonVisible = false;
        [RelayCommand]
        protected void NextWaterChanged()
        {
            NextWaterUndoButtonVisible = (!IsInitialization
                                                                            && !IsBeingUndone
                                                                            && (AlterPlant.DateOfNextWatering != InitialPlant.DateOfNextWatering
                                                                            || AlterPlant.TimeOfNextWatering != InitialPlant.TimeOfNextWatering))
                                                                            ? true : false;
        }
        public string NextWateringUnsavedChangesWarning => NextWaterUndoButtonVisible ? "• Next Watering\n" : "";
        [RelayCommand]
        protected void NextWaterChangedSectionUndo()
        {
            IsBeingUndone = true;
            HumidityChangedSectionUndo();
            TemperatureChangedSectionUndo();
            AlterPlant.DateOfNextWatering = InitialPlant.DateOfNextWatering;
            AlterPlant.TimeOfNextWatering = InitialPlant.TimeOfNextWatering;
            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            NextWaterUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MistIntervalUnsavedChangesWarning))]
        protected bool mistIntervalUndoButtonVisible = false;
        [RelayCommand]
        protected void MistIntervalChanged() 
        { 
            MistIntervalUndoButtonVisible = (!IsInitialization && !IsBeingUndone && (AlterPlant.MistInterval != InitialPlant.MistInterval)) ? true : false;
            if (!IsBeingAutoAdjusted && !IsInitialization && !IsBeingUndone)
            {
                 AlterPlant.BaseMistIntervalForTempAndHum = AlterPlant.FindBase((AlterPlant.DateOfNextMisting.Date - AlterPlant.DateOfLastMisting.Date).Days);
            }
        } 
        public string MistIntervalUnsavedChangesWarning => MistIntervalUndoButtonVisible ? "• Mist Interval\n" : "";
        [RelayCommand]
        protected void MistIntervalChangedSectionUndo()
        {
            IsBeingUndone = true;
            HumidityChangedSectionUndo();
            TemperatureChangedSectionUndo();
            AlterPlant.MistInterval = InitialPlant.MistInterval;
            MistDaysFromNow = AlterPlant.MistInterval != null ? (int)AlterPlant.MistInterval : (AlterPlant.DateOfNextMisting.Date - AlterPlant.DateOfLastMisting.Date).Days;
            IsInitialization = true;
            MistIntervalPickerValue = MistingInterval.FirstOrDefault(x => x.NumFromNow == InitialMistDaysFromNow);
            MistIntervalPickerValue ??= MistingInterval.First(x => x.NumFromNow == -1);
            IsInitialization = false;
            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            MistIntervalUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LastMistingUnsavedChangesWarning))]
        protected bool lastMistedUndoButtonVisible = false;
        [RelayCommand]
        protected void LastMistedChanged()
        {
            LastMistedUndoButtonVisible = (!IsInitialization
                                            && !IsBeingUndone
                                            && (AlterPlant.DateOfLastMisting != InitialPlant.DateOfLastMisting
                                            || AlterPlant.TimeOfLastMisting != InitialPlant.TimeOfLastMisting))
                                            ? true : false;
        }
        public string LastMistingUnsavedChangesWarning => LastMistedUndoButtonVisible ? "• Last Misting\n" : "";
        [RelayCommand]
        protected void LastMistedChangedSectionUndo()
        {
            IsBeingUndone = true;
            HumidityChangedSectionUndo();
            TemperatureChangedSectionUndo();
            AlterPlant.DateOfLastMisting = InitialPlant.DateOfLastMisting;
            AlterPlant.TimeOfLastMisting = InitialPlant.TimeOfLastMisting;

            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            LastMistedUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NextMistingUnsavedChangesWarning))]
        protected bool nextMistUndoButtonVisible = false;
        [RelayCommand]
        protected void NextMistChanged()
        {
            NextMistUndoButtonVisible = (!IsInitialization
                                        && !IsBeingUndone
                                        && (AlterPlant.DateOfNextMisting != InitialPlant.DateOfNextMisting
                                        || AlterPlant.TimeOfNextMisting != InitialPlant.TimeOfNextMisting))
                                        ? true : false;
        }
        public string NextMistingUnsavedChangesWarning => NextMistUndoButtonVisible ? "• Next Misting\n" : "";
        [RelayCommand]
        protected void NextMistChangedSectionUndo()
        {
            IsBeingUndone = true;
            HumidityChangedSectionUndo();
            TemperatureChangedSectionUndo();
            AlterPlant.DateOfNextMisting = InitialPlant.DateOfNextMisting;
            AlterPlant.TimeOfNextMisting = InitialPlant.TimeOfNextMisting;

            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            NextMistUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MoveIntervalUnsavedChangesWarning))]
        protected bool moveIntervalUndoButtonVisible = false;
        [RelayCommand]
        protected void MoveIntervalChanged() { MoveIntervalUndoButtonVisible = (!IsInitialization && !IsBeingUndone) ? true : false; }
        public string MoveIntervalUnsavedChangesWarning => MoveIntervalUndoButtonVisible ? "• Moving Interval\n" : "";
        [RelayCommand]
        protected void MoveIntervalChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.SunInterval = InitialPlant.SunInterval;
            SunDaysFromNow = AlterPlant.SunInterval != null ? (int)AlterPlant.SunInterval : (AlterPlant.DateOfNextMove.Date - AlterPlant.DateOfLastMove.Date).Days;
            SunIntervalPickerValue = SunInterval.FirstOrDefault(x => x.NumFromNow == InitialSunDaysFromNow);
            SunIntervalPickerValue ??= SunInterval.First(x => x.NumFromNow == -1);

            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            MoveIntervalUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LastMovedUnsavedChangesWarning))]
        protected bool lastMovedUndoButtonVisible = false;
        [RelayCommand]
        protected void LastMovedChanged()
        {
            LastMovedUndoButtonVisible = (!IsInitialization
                                            && !IsBeingUndone
                                            && (AlterPlant.DateOfLastMove != InitialPlant.DateOfLastMove
                                            || AlterPlant.TimeOfLastMove != InitialPlant.TimeOfLastMove))
                                            ? true : false;
        }
        public string LastMovedUnsavedChangesWarning => LastMovedUndoButtonVisible ? "• Last Move\n" : "";
        [RelayCommand]
        protected void LastMovedChangedSectionUndo()
        {
            IsBeingUndone = true;

            AlterPlant.DateOfLastMove = InitialPlant.DateOfLastMove;
            AlterPlant.TimeOfLastMove = InitialPlant.TimeOfLastMove;

            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            LastMovedUndoButtonVisible = false;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NextMoveUnsavedChangesWarning))]
        protected bool nextMoveUndoButtonVisible = false;
        [RelayCommand]
        protected void NextMoveChanged()
        {
            NextMoveUndoButtonVisible = (!IsInitialization
                                        && !IsBeingUndone
                                        && (AlterPlant.DateOfNextMove != InitialPlant.DateOfNextMove
                                        || AlterPlant.TimeOfNextMove != InitialPlant.TimeOfNextMove))
                                        ? true : false;
        }
        public string NextMoveUnsavedChangesWarning => NextMoveUndoButtonVisible ? "• Next Move\n" : "";
        [RelayCommand]
        protected void NextMoveChangedSectionUndo()
        {
            IsBeingUndone = true;

            AlterPlant.DateOfNextMove = InitialPlant.DateOfNextMove;
            AlterPlant.TimeOfNextMove = InitialPlant.TimeOfNextMove;

            OnPropertyChanged(nameof(AlterPlant));
            IsBeingUndone = false;
            NextMoveUndoButtonVisible = false;
        }

        //Not sure if this one is ready for prime time yet :(
        [RelayCommand]
        protected void UndoAll()
        {
            IsBeingUndone = true;
            //AlterPlant = new Plant(InitialPlant); This is causing visual bugs
            OnPropertyChanged(nameof(AlterPlant));

            HumidityChangedSectionUndo();
            TemperatureChangedSectionUndo();
            GroupSectionUndo();
            ImageChangedSectionUndo();
            SpeciesChangedSectionUndo();
            GivenNameChangedSectionUndo();
            DobChangedSectionUndo();
            WaterIntervalChangedSectionUndo();
            LastWateredChangedSectionUndo();
            NextWaterChangedSectionUndo();
            MistIntervalChangedSectionUndo();
            LastMistedChangedSectionUndo();
            NextMistChangedSectionUndo();
            MoveIntervalChangedSectionUndo();
            LastMovedChangedSectionUndo();
            NextMoveChangedSectionUndo();
            IsBeingUndone = false;
        }





        [ObservableProperty]
        public bool shouldGetNewData = false;

        [ObservableProperty]
        public bool shouldGetNewGroupData = false;

        [ObservableProperty]
        public bool isImageSelected = false;

        public Plant InitialPlant = new();

        private readonly Random rand = new();

        [ObservableProperty]
        public List<GroupColors> groupColors = PickerService.GetSelectableColors();

        [ObservableProperty]
        public GroupColors selectedGroupColor;

        [ObservableProperty]
        public Plant alterPlant = new();

        [ObservableProperty]
        public List<ClipetSpeechBubble> speechBubbles;

        public ObservableCollection<PlantGroup> PlantGroups { get; set; } = [];

        public PlantDetailsViewModel(IDatabaseService databaseService)
        {
            IsInitialization = true;
            _databaseService = databaseService;
            wateringInterval = PickerService.GetWaterIntervals();
            mistingInterval = PickerService.GetWaterIntervals();
            sunInterval = PickerService.GetWaterIntervals();
            HumidityIntervals = PickerService.GetHumidityPercent();
            TemperatureIntervalsF = PickerService.GetTemperatureF();
            TemperatureIntervalsC = PickerService.GetTemperatureC();
            SelectedGroupColor = GroupColors[rand.Next(GroupColors.Count)];
            HumidityIntervalPickerValueDetails = HumidityIntervals.FirstOrDefault(_ => _.HumidityLevel == Preferences.Default.Get("humidity_level", 44)) ?? new HumidityInterval() { HumidityLevel = 30, IntervalText = "Normal Indoor Humidity" };

            IsCelsius = Preferences.Default.Get("is_Celsius", false);
            TemperatureIntervalPickerValueCDetails = TemperatureIntervalsC.FirstOrDefault(_ => _.TemperatureLevel == Preferences.Default.Get("temperature_level", 60)) ?? new TemperatureInterval() { TemperatureLevel = 60, IntervalText = "Normal Indoor Temperatures", IsCelsius = true };
            TemperatureIntervalPickerValueCDetails.TemperatureLevel = TemperatureIntervalPickerValueCDetails.TemperatureLevel;
            TemperatureIntervalPickerValueFDetails = TemperatureIntervalsF.FirstOrDefault(_ => _.TemperatureLevel == Preferences.Default.Get("temperature_level", 60)) ?? new TemperatureInterval() { TemperatureLevel = 60, IntervalText = "Normal Indoor Temperatures", IsCelsius = false};
            TemperatureIntervalPickerValueFDetails.TemperatureLevel = TemperatureIntervalPickerValueFDetails.TemperatureLevel;
            IsInitialization = false;
        }

        public void CorrectlySizeTimePickerBoxes()
        {
            AlterPlant.TimeOfLastMisting = new TimeSpan(0, 0, 0);
            AlterPlant.TimeOfLastWatering = new TimeSpan(0, 0, 0);
            AlterPlant.TimeOfLastMove = new TimeSpan(0, 0, 0);
            AlterPlant.TimeOfNextMisting = new TimeSpan(0, 0, 0);
            AlterPlant.TimeOfNextWatering = new TimeSpan(0, 0, 0);
            AlterPlant.TimeOfNextMove = new TimeSpan(0, 0, 0);

            AlterPlant.DateOfLastWatering = new DateTime(2020, 12, 25);
            AlterPlant.DateOfNextWatering = new DateTime(2020, 12, 25);
            AlterPlant.DateOfLastMisting = new DateTime(2020, 12, 25);
            AlterPlant.DateOfNextMisting = new DateTime(2020, 12, 25);
            AlterPlant.DateOfLastMove = new DateTime(2020, 12, 25);
            AlterPlant.DateOfNextMove = new DateTime(2020, 12, 25);

            AlterPlant.TimeOfLastMisting = InitialPlant.TimeOfLastMisting;
            AlterPlant.TimeOfLastWatering = InitialPlant.TimeOfLastWatering;
            AlterPlant.TimeOfLastMove = InitialPlant.TimeOfLastMove;
            AlterPlant.TimeOfNextMisting = InitialPlant.TimeOfNextMisting;
            AlterPlant.TimeOfNextWatering = InitialPlant.TimeOfNextWatering;
            AlterPlant.TimeOfNextMove = InitialPlant.TimeOfNextMove;

            AlterPlant.DateOfLastWatering = InitialPlant.DateOfLastWatering;
            AlterPlant.DateOfNextWatering = InitialPlant.DateOfNextWatering;
            AlterPlant.DateOfLastMisting = InitialPlant.DateOfLastMisting;
            AlterPlant.DateOfNextMisting = InitialPlant.DateOfNextMisting;
            AlterPlant.DateOfLastMove = InitialPlant.DateOfLastMove;
            AlterPlant.DateOfNextMove = InitialPlant.DateOfNextMove;
        }

        [RelayCommand]
        public async Task AppearingAsync()
        {
            IsInitialization = true;
            await GetPlantsAsync();
            var isNew = DataPlants.Select(_ => _.GivenName).Contains(InitialPlant.GivenName);
            if (isNew) { SaveText = "Update"; } else { SaveText = "Add"; }
            PlantSuggestions = PlantSuggestions.Count > 0 ? PlantSuggestions : new(await _databaseService.GetAllAutofillPlantAsync());
            CorrectlySizeTimePickerBoxes();
            // extract to its own reusable method with reflection DRY!
            GroupPickerValue = AlterPlant.PlantGroupName != null ? PlantGroups.FirstOrDefault(_ => _.GroupName == AlterPlant.PlantGroupName) : PlantGroups.FirstOrDefault(_ => _.GroupName == "Ungrouped");
            

            InitialWaterDaysFromNow = WaterDaysFromNow = AlterPlant.WaterInterval != null ? (int)AlterPlant.WaterInterval : (AlterPlant.DateOfNextWatering.Date - AlterPlant.DateOfLastWatering.Date).Days;
            WaterIntervalPickerValue = WateringInterval.FirstOrDefault(x => x.NumFromNow == WaterDaysFromNow);
            WaterIntervalPickerValue ??= WateringInterval.First(x => x.NumFromNow == -1);

            InitialMistDaysFromNow = MistDaysFromNow = AlterPlant.MistInterval != null ? (int)AlterPlant.MistInterval : (AlterPlant.DateOfNextMisting.Date - AlterPlant.DateOfLastMisting.Date).Days;
            MistIntervalPickerValue = MistingInterval.FirstOrDefault(x => x.NumFromNow == MistDaysFromNow);
            MistIntervalPickerValue ??= MistingInterval.First(x => x.NumFromNow == -1);
            InitialSunDaysFromNow = SunDaysFromNow = AlterPlant.SunInterval != null ? (int)AlterPlant.SunInterval : (AlterPlant.DateOfNextMove.Date - AlterPlant.DateOfLastMove.Date).Days;
            SunIntervalPickerValue = SunInterval.FirstOrDefault(x => x.NumFromNow == SunDaysFromNow);
            SunIntervalPickerValue ??= SunInterval.First(x => x.NumFromNow == -1);

            WaterGridText = AlterPlant.UseWatering ? "Do Not Use Watering" : "Use Watering";
            MistGridText = AlterPlant.UseMisting ? "Do Not Use Misting" : "Use Misting";
            SunGridText = AlterPlant.UseMoving ? "Do Not Use Sunlight Move" : "Use Sunlight Move";

            IsCelsius = Preferences.Default.Get("is_Celsius", false);
            AlterPlant.TemperatureIntervalInt = AlterPlant.TemperatureIntervalInt ?? TemperatureIntervalPickerValueFDetails.TemperatureLevel;
            TemperatureIntervalPickerValueFDetails = TemperatureIntervalsF.FirstOrDefault(_ => _.TemperatureLevel == (int)AlterPlant.TemperatureIntervalInt);
            TemperatureIntervalPickerValueCDetails = TemperatureIntervalsC.FirstOrDefault(_ => _.TemperatureLevel == (int)AlterPlant.TemperatureIntervalInt);
            TemperatureIntervalPickerValueCDetails.TemperatureLevel = TemperatureIntervalPickerValueCDetails.TemperatureLevel;
            TemperatureIntervalPickerValueFDetails.TemperatureLevel = TemperatureIntervalPickerValueFDetails.TemperatureLevel;
            AlterPlant.HumanityIntervalInt = AlterPlant.HumanityIntervalInt ?? HumidityIntervalPickerValueDetails.HumidityLevel;
            OnPropertyChanged(nameof(TemperatureIntervalPickerValueFDetails));
            OnPropertyChanged(nameof(TemperatureIntervalPickerValueCDetails));
            HumidityIntervalPickerValueDetails = HumidityIntervals.FirstOrDefault(_ => _.HumidityLevel == (int)AlterPlant.HumanityIntervalInt);
            OnPropertyChanged(nameof(HumidityIntervalPickerValueDetails));
            IsInitialization = false;
            if (AlterPlant.BaseWaterIntervalForTempAndHum is null && AlterPlant.UseWatering)
            {
                AlterPlant.BaseWaterIntervalForTempAndHum = (InitialPlant.DateOfNextWatering.Date - InitialPlant.DateOfLastWatering.Date).Days;
                
                HumidityValueChanged(HumidityIntervalPickerValueDetails);
                TemperatureFValueChanged(TemperatureIntervalPickerValueFDetails);
                TemperatureCValueChanged(TemperatureIntervalPickerValueCDetails);
            } else if (AlterPlant.UseWatering)
            {
                HumidityValueChanged(HumidityIntervalPickerValueDetails);
                TemperatureFValueChanged(TemperatureIntervalPickerValueFDetails);
                TemperatureCValueChanged(TemperatureIntervalPickerValueCDetails);
            }
           
            if (AlterPlant.BaseMistIntervalForTempAndHum is null && AlterPlant.UseMisting)
            {
                AlterPlant.BaseMistIntervalForTempAndHum = (InitialPlant.DateOfNextMisting.Date - InitialPlant.DateOfLastMisting.Date).Days;
                HumidityValueChanged(HumidityIntervalPickerValueDetails);
                TemperatureFValueChanged(TemperatureIntervalPickerValueFDetails);
                TemperatureCValueChanged(TemperatureIntervalPickerValueCDetails);
            }
            else if (AlterPlant.UseMisting)
            {
                HumidityValueChanged(HumidityIntervalPickerValueDetails);
                TemperatureFValueChanged(TemperatureIntervalPickerValueFDetails);
                TemperatureCValueChanged(TemperatureIntervalPickerValueCDetails);
            }
            IsInitialization = true;

            if (InitialPlant == AlterPlant)
            {
                ShouldGetNewData = false;
                ShouldGetNewGroupData = false;
            }

            if (AlterPlant.ImageLocation is not null)
            {
                SetImageSourceOfPlant();
                IsImageSelected = true;
            }
            else
            {
                IsImageSelected = false;
            }
            OnHumidityIntervalPickerValueDetailsChanged(HumidityIntervalPickerValue);
            OnTemperatureIntervalPickerValueFDetailsChanged(TemperatureIntervalPickerValueF);
            OnPropertyChanged(nameof(AlterPlant));
            IsInitialization = false;
        }

        [RelayCommand]
        public void SpeciesSearchAction(string searchQuery)
        {
            if (ShowSearchSuggestionsBox)
            {
                QueryAutofillPlantAsyncFromSearch(searchQuery);
            }
        }

        [RelayCommand]
        public async Task AutoFillPlantSpeciesAsync(SearchedPlants searchedPlant)
        {
            var accept = true;
            var message = WaterIntervalUnsavedChangesWarning + LastWateredUnsavedChangesWarning + NextWateringUnsavedChangesWarning +
                MistIntervalUnsavedChangesWarning + LastMistingUnsavedChangesWarning + NextMistingUnsavedChangesWarning + MoveIntervalUnsavedChangesWarning +
                LastMovedUnsavedChangesWarning + NextMoveUnsavedChangesWarning;
            if (!string.IsNullOrEmpty(message))
            {
                accept = await Application.Current.MainPage.DisplayAlert("You have a pending plant! Are you sure you want to autofill this new plant species?", message, "Do It!", "Please Don't");
            }
            if (accept)
            {
                var saveName = AlterPlant.GivenName;
                var saveGroup = AlterPlant.PlantGroupName;
                var saveGroupColorHexString = AlterPlant.GroupColorHexString;
                var saveImage = AlterPlant.PlantImageSource;
                var saveImageLocation = AlterPlant.ImageLocation;
                var saveDOB = AlterPlant.DateOfBirth;
                var saveId = AlterPlant.Id;

                AlterPlant = new Plant(searchedPlant);
                
                AlterPlant.GivenName = saveName;
                AlterPlant.PlantGroupName = saveGroup;
                AlterPlant.GroupColorHexString = saveGroupColorHexString;
                AlterPlant.PlantImageSource = saveImage;
                AlterPlant.ImageLocation = saveImageLocation;
                AlterPlant.DateOfBirth = saveDOB;
                AlterPlant.Id = saveId;
                AlterPlant.BaseWaterIntervalForTempAndHum = (AlterPlant.DateOfNextWatering.Date - AlterPlant.DateOfLastWatering.Date).Days;
                AlterPlant.BaseMistIntervalForTempAndHum = (AlterPlant.DateOfNextMisting.Date - AlterPlant.DateOfLastMisting.Date).Days;
                HumidityValueChanged(HumidityIntervalPickerValueDetails);
                TemperatureFValueChanged(TemperatureIntervalPickerValueFDetails);
                TemperatureCValueChanged(TemperatureIntervalPickerValueCDetails);
                WaterDaysFromNowChanged(WaterDaysFromNow);
                MistDaysFromNowChanged(MistDaysFromNow);
                OnPropertyChanged(nameof(AlterPlant));

                HideSearchSuggestionBox();
            }
        }

        [RelayCommand]
        public async Task GetPlantsAsync(bool shouldSort = false)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var plants = await _databaseService.GetAllPlantAsync();

                // checking if there are really 0 plants or if an issue occured.
                if (plants.Count == 0)
                {
                    plants = await _databaseService.GetAllPlantAsync();
                    // small buffer for plant count
                    for (var i = 0; i < 2; i++)
                    {
                        if (plants.Count != 0)
                        {
                            break;
                        }
                    }
                }
                DataPlants = new ObservableCollection<Plant>(plants);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
            return;
        }

        public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsInitialization = true;
            AlterPlant = query["Plant"] as Plant;
            InitialPlant = new Plant(AlterPlant);
            OnPropertyChanged(nameof(AlterPlant));
            var groups = query["PlantGroup"] as List<PlantGroup>;
            PlantGroups = new ObservableCollection<PlantGroup>(groups);
            OnPropertyChanged(nameof(PlantGroups));
            IsInitialization = false;
        }
        [RelayCommand]
        public async Task ValidateAsync()
        {
            try
            {
                if (!IsInitialization && AlterPlant is not null)
            {
                    await ValidateAlterPlantAsync();
                }
            }
            catch (Exception ex)
            {
                return;
            }
            return;

        }

        [ObservableProperty]
        private bool isInitialization;

        [ObservableProperty]
        public bool addNewGroupGridVisible = false;

        [ObservableProperty]
        public string addGroupButtonText = "+";

        [ObservableProperty]
        public PlantGroup groupPickerValue;

        [ObservableProperty]
        protected List<Interval> wateringInterval;

        [ObservableProperty]
        protected List<Interval> mistingInterval;

        [ObservableProperty]
        protected List<Interval> sunInterval;

        [ObservableProperty]
        public bool customWaterIntervalGridVisible = false;

        [ObservableProperty]
        public bool customMistIntervalGridVisible = false;

        [ObservableProperty]
        public bool customSunIntervalGridVisible = false;

        [ObservableProperty]
        public bool waterGridVisible = false;

        [ObservableProperty]
        public string waterGridText = "Use Watering";

        [ObservableProperty]
        public bool mistGridVisible = false;

        [ObservableProperty]
        public string mistGridText = "Use Misting";

        [ObservableProperty]
        public bool sunGridVisible = false;

        [ObservableProperty]
        public string sunGridText = "Use Sunlight Move";

        [ObservableProperty]
        public Interval waterIntervalPickerValue;

        [ObservableProperty]
        public Interval mistIntervalPickerValue;

        [ObservableProperty]
        public Interval sunIntervalPickerValue;

        [ObservableProperty]
        public double waterDaysFromNow;

        [ObservableProperty]
        public double mistDaysFromNow;

        [ObservableProperty]
        public double sunDaysFromNow;

        [RelayCommand]
        private async Task ImageOfPlantToBase64Async()
        {
            try
            {
                AlterPlant.ImageLocation = await Base64ImageConverterService.PickedImageToBase64Async();
                OnPropertyChanged(nameof(AlterPlant));
                SetImageSourceOfPlant();
            } 
            catch(Exception ex) 
            {
                AlterPlant.ImageLocation = AlterPlant.ImageLocation;
                OnPropertyChanged(nameof(AlterPlant));
            }
            
        }

        [RelayCommand]
        public void SetImageSourceOfPlant()
        {
            AlterPlant.PlantImageSource = Base64ImageConverterService.Base64ToImage(AlterPlant.ImageLocation);
            IsImageSelected = AlterPlant.PlantImageSource is not null ? true : false;
            CurrentPlantImage = AlterPlant.PlantImageSource;
            ImageChanged();
            OnPropertyChanged(nameof(AlterPlant));
            OnPropertyChanged(nameof(IsImageSelected));
        }

        partial void OnGroupPickerValueChanged(PlantGroup value)
        {
            if (!IsInitialization)
            {
                AlterPlant.PlantGroupName = value.GroupName;
                AlterPlant.GroupColorHexString = value.GroupColorHex;
                OnPropertyChanged(nameof(AlterPlant));
            }
        }

        partial void OnWaterDaysFromNowChanged(double value)
        {
            WaterDaysFromNowChanged(value);
        }

        private void WaterDaysFromNowChanged(double value)
        {
            if (!IsInitialization)
            {
                AlterPlant.DateOfNextWatering = AlterPlant.DateOfLastWatering.AddDays(value);
            }

            AlterPlant.WaterInterval = value;
            OnPropertyChanged(nameof(AlterPlant));
        }

        partial void OnWaterIntervalPickerValueChanged(Interval value)
        {
            if (value is null || value.NumFromNow == -1)
            {
                CustomWaterIntervalGridVisible = true;
            }
            else
            {
                CustomWaterIntervalGridVisible = false;
                WaterDaysFromNow = value.NumFromNow;
            }
        }

        partial void OnMistDaysFromNowChanged(double value)
        {
            MistDaysFromNowChanged(value);
        }

        private void MistDaysFromNowChanged(double value)
        {
            if (!IsInitialization)
            {
                AlterPlant.DateOfNextMisting = AlterPlant.DateOfLastMisting.AddDays(value);
            }

            AlterPlant.MistInterval = value;
            OnPropertyChanged(nameof(AlterPlant));
        }

        partial void OnMistIntervalPickerValueChanged(Interval value)
        {
            if (value is null || value.NumFromNow == -1)
            {
                CustomMistIntervalGridVisible = true;
            }
            else
            {
                CustomMistIntervalGridVisible = false;
                MistDaysFromNow = value.NumFromNow;
            }
        }

        partial void OnSunDaysFromNowChanged(double value)
        {
            if (!IsInitialization)
            {
                AlterPlant.DateOfNextMove = AlterPlant.DateOfLastMove.AddDays(value);
            }

            AlterPlant.SunInterval = value;
            OnPropertyChanged(nameof(AlterPlant));
        }

        partial void OnSunIntervalPickerValueChanged(Interval value)
        {
            if (value is null || value.NumFromNow == -1)
            {
                CustomSunIntervalGridVisible = true;
            }
            else
            {
                CustomSunIntervalGridVisible = false;
                SunDaysFromNow = value.NumFromNow;
            }
        }

        public Plant SetPlantValues(Plant plant)
        {
            if (WaterDaysFromNow != -1)
            {
                plant.WaterInterval = WaterDaysFromNow;
            }

            return plant;
        }

        [RelayCommand]
        public async Task SyncWaterAndMistAsync()
        {
            if (!IsInitialization && AlterPlant.UseMisting && AlterPlant.UseWatering)
            {
                var shouldSync = await Application.Current.MainPage.DisplayAlert("OH HOLD ON!", $"This will make watering and misting occur at the same date and time.", "Do it!", "DECLINE");
                if (shouldSync)
                {
                    AlterPlant.BaseMistIntervalForTempAndHum = AlterPlant.BaseWaterIntervalForTempAndHum;
                    AlterPlant.DateOfLastMisting = AlterPlant.DateOfLastWatering;
                    AlterPlant.TimeOfLastMisting = AlterPlant.TimeOfLastWatering;
                    AlterPlant.MistInterval = AlterPlant.WaterInterval;
                    MistDaysFromNow = WaterDaysFromNow;
                    IsInitialization = true;
                    MistIntervalPickerValue = WaterIntervalPickerValue;
                    MistIntervalPickerValue ??= MistingInterval.First(x => x.NumFromNow == -1);
                    IsInitialization = false;
                    AlterPlant.DateOfNextMisting = AlterPlant.DateOfNextWatering;
                    AlterPlant.TimeOfNextMisting = AlterPlant.TimeOfNextWatering;
                }
            }
        }

        [RelayCommand]
        protected void AddGroupShowPressed()
        {
            AddNewGroupGridVisible = !AddNewGroupGridVisible;
            AddGroupButtonText = AddNewGroupGridVisible ? "-" : "+";
        }

        [RelayCommand]
        protected async Task AddNewGroupAsync(string newPlantGroupName)
        {
            var newPlantGroup = new PlantGroup()
            {
                GroupId = PlantGroups.Any() ? PlantGroups.Max(x => x.GroupId) + 1 : 0,
                GroupName = newPlantGroupName,
                GroupColorHex = $"{SelectedGroupColor.ColorsHex}",
            };
            var message = NewGroupValidation(newPlantGroup);
            if (string.IsNullOrEmpty(message))
            {
                var result = await AddUpdateGroupAsync(newPlantGroup);
                if (result)
                {
                    PlantGroups.Add(newPlantGroup);
                    OnPropertyChanged(nameof(PlantGroups));
                    AddGroupShowPressed();
                    GroupPickerValue = newPlantGroup;
                }
            } else
            {
                await Application.Current.MainPage.DisplayAlert("OH HOLD ON!", message, "Gotcha");
            }
            
        }

        [RelayCommand]
        protected void ClearImage()
        {
            AlterPlant.ImageLocation = null;
            AlterPlant.PlantImageSource = null;
            IsImageSelected = false;
            OnPropertyChanged(nameof(AlterPlant));
        }

        [RelayCommand]
        protected void UseWateringPressed(bool value)
        {
            AlterPlant.UseWatering = !AlterPlant.UseWatering;
            WaterGridText = AlterPlant.UseWatering ? "Do Not Use Watering" : "Use Watering";
            OnPropertyChanged(nameof(AlterPlant));
        }

        [RelayCommand]
        protected void UseMistingPressed(bool value)
        {
            AlterPlant.UseMisting = !AlterPlant.UseMisting;
            MistGridText = AlterPlant.UseMisting ? "Do Not Use Misting" : "Use Misting";
            OnPropertyChanged(nameof(AlterPlant));
        }

        [RelayCommand]
        protected void UseSunPressed(bool value)
        {
            AlterPlant.UseMoving = !AlterPlant.UseMoving;
            SunGridText = AlterPlant.UseMoving ? "Do Not Use Sunlight Move" : "Use Sunlight Move";
            OnPropertyChanged(nameof(AlterPlant));
        }

        [RelayCommand]
        protected async Task<bool> AddUpdateGroupAsync(PlantGroup plantGroup)
        {
            var result = false;
            if (IsBusy)
                return result;

            try
            {
                //plant = SetPlantValues(plant);

                IsBusy = true;
                await _databaseService.AddUpdateNewPlantGroupAsync(plantGroup);
                result = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to add or update plant group: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
                result = false;
            }
            finally
            {
                IsBusy = false;
                FriendlyLabel = _databaseService.StatusMessage;
                ShouldGetNewGroupData = true;
                await FriendlyLabelToastAsync();

            }
            return result;
        }

        public async Task FillUnsafePlantsAsync()
        {
            var plants = await _databaseService.GetAllPlantAsync();
            var plantNames = plants.Select(_ => _.GivenName);
            UnsafePlantNames = plantNames.Where(_ => _ != InitialPlant.GivenName).ToList();
        }

        partial void OnHumidityIntervalPickerValueDetailsChanged(HumidityInterval value)
        {
            value = HumidityValueChanged(value);
        }

        private HumidityInterval HumidityValueChanged(HumidityInterval value)
        {
            IsBeingAutoAdjusted = true;
            if (value is null) { value = HumidityIntervals.FirstOrDefault(_ => _.HumidityLevel == Preferences.Default.Get("Humidity_level", 44)); }
            if (!IsInitialization && !IsBeingUndone)
            {
                AlterPlant.HumanityIntervalInt = value.HumidityLevel;
                HumidityChanged();
                HumidityIntervalPickerValueDetails = value;
                if (AlterPlant.BaseWaterIntervalForTempAndHum is not null) { AlterPlant.WaterInterval = AlterPlant.FindCurrent((double)AlterPlant.BaseWaterIntervalForTempAndHum); }
                if (AlterPlant.BaseMistIntervalForTempAndHum is not null) { AlterPlant.MistInterval = AlterPlant.FindCurrent((double)AlterPlant.BaseMistIntervalForTempAndHum); }

                WaterDaysFromNow = AlterPlant.WaterInterval != null ? (int)AlterPlant.WaterInterval : (AlterPlant.DateOfNextWatering.Date - AlterPlant.DateOfLastWatering.Date).Days;
                WaterIntervalPickerValue = WateringInterval.FirstOrDefault(x => x.NumFromNow == WaterDaysFromNow);
                WaterIntervalPickerValue ??= WateringInterval.First(x => x.NumFromNow == -1);
                MistDaysFromNow = AlterPlant.MistInterval != null ? (int)AlterPlant.MistInterval : (AlterPlant.DateOfNextMisting.Date - AlterPlant.DateOfLastMisting.Date).Days;
                MistIntervalPickerValue = MistingInterval.FirstOrDefault(x => x.NumFromNow == MistDaysFromNow);
                MistIntervalPickerValue ??= MistingInterval.First(x => x.NumFromNow == -1);
                OnPropertyChanged(nameof(AlterPlant));
            }
            IsBeingAutoAdjusted = false;
            return value;
        }

        partial void OnTemperatureIntervalPickerValueFDetailsChanged(TemperatureInterval value)
        {
            value = TemperatureFValueChanged(value);
        }

        private TemperatureInterval TemperatureFValueChanged(TemperatureInterval value)
        {
            IsBeingAutoAdjusted = true;
            if (value is null) { value = TemperatureIntervalsF.FirstOrDefault(_ => _.TemperatureLevel == Preferences.Default.Get("Temperature_level", 60)); }
            if (!IsChangingCtoF && !IsInitialization && !IsBeingUndone)
            {
                IsChangingCtoF = true;
                if (TemperatureIntervalPickerValueFDetails is null) { TemperatureIntervalPickerValueFDetails = value; }
                AlterPlant.TemperatureIntervalInt = value.TemperatureLevel;
                TemperatureIntervalPickerValueFDetails.TemperatureLevel = TemperatureIntervalPickerValueFDetails.TemperatureLevel;
                TemperatureChanged();
                if (AlterPlant.BaseWaterIntervalForTempAndHum is not null) { AlterPlant.WaterInterval = AlterPlant.FindCurrent((double)AlterPlant.BaseWaterIntervalForTempAndHum); }
                if (AlterPlant.BaseMistIntervalForTempAndHum is not null) { AlterPlant.MistInterval = AlterPlant.FindCurrent((double)AlterPlant.BaseMistIntervalForTempAndHum); }
                WaterDaysFromNow = AlterPlant.WaterInterval != null ? (int)AlterPlant.WaterInterval : (AlterPlant.DateOfNextWatering.Date - AlterPlant.DateOfLastWatering.Date).Days;
                WaterIntervalPickerValue = WateringInterval.FirstOrDefault(x => x.NumFromNow == WaterDaysFromNow);
                WaterIntervalPickerValue ??= WateringInterval.First(x => x.NumFromNow == -1);
                MistDaysFromNow = AlterPlant.MistInterval != null ? (int)AlterPlant.MistInterval : (AlterPlant.DateOfNextMisting.Date - AlterPlant.DateOfLastMisting.Date).Days;
                MistIntervalPickerValue = MistingInterval.FirstOrDefault(x => x.NumFromNow == MistDaysFromNow);
                MistIntervalPickerValue ??= MistingInterval.First(x => x.NumFromNow == -1);
                OnPropertyChanged(nameof(AlterPlant));
            }
            IsChangingCtoF = false;
            IsBeingAutoAdjusted = false;
            return value;
        }

        partial void OnTemperatureIntervalPickerValueCDetailsChanged(TemperatureInterval value)
        {
            value = TemperatureCValueChanged(value);
        }

        private TemperatureInterval TemperatureCValueChanged(TemperatureInterval value)
        {
            IsBeingAutoAdjusted = true;
            if (value is null) { value = TemperatureIntervalsC.FirstOrDefault(_ => _.TemperatureLevel == Preferences.Default.Get("Temperature_level", 60)); }
            if (!IsChangingCtoF && !IsInitialization && !IsBeingUndone)
            {
                IsChangingCtoF = true;
                if (TemperatureIntervalPickerValueCDetails is null) { TemperatureIntervalPickerValueCDetails.IsCelsius = true; TemperatureIntervalPickerValueCDetails = value; }
                AlterPlant.TemperatureIntervalInt = value.TemperatureLevel;
                TemperatureIntervalPickerValueCDetails.TemperatureLevel = TemperatureIntervalPickerValueCDetails.TemperatureLevel;
                TemperatureChanged();
                if (AlterPlant.BaseWaterIntervalForTempAndHum is not null) { AlterPlant.WaterInterval = AlterPlant.FindCurrent((double)AlterPlant.BaseWaterIntervalForTempAndHum); }
                if (AlterPlant.BaseMistIntervalForTempAndHum is not null) { AlterPlant.MistInterval = AlterPlant.FindCurrent((double)AlterPlant.BaseMistIntervalForTempAndHum); }
                if (AlterPlant.BaseWaterIntervalForTempAndHum is not null) { AlterPlant.WaterInterval = AlterPlant.FindCurrent((double)AlterPlant.BaseWaterIntervalForTempAndHum); }
                if (AlterPlant.BaseMistIntervalForTempAndHum is not null) { AlterPlant.MistInterval = AlterPlant.FindCurrent((double)AlterPlant.BaseMistIntervalForTempAndHum); }
                WaterDaysFromNow = AlterPlant.WaterInterval != null ? (int)AlterPlant.WaterInterval : (AlterPlant.DateOfNextWatering.Date - AlterPlant.DateOfLastWatering.Date).Days;
                WaterIntervalPickerValue = WateringInterval.FirstOrDefault(x => x.NumFromNow == WaterDaysFromNow);
                WaterIntervalPickerValue ??= WateringInterval.First(x => x.NumFromNow == -1);
                MistDaysFromNow = AlterPlant.MistInterval != null ? (int)AlterPlant.MistInterval : (AlterPlant.DateOfNextMisting.Date - AlterPlant.DateOfLastMisting.Date).Days;
                MistIntervalPickerValue = MistingInterval.FirstOrDefault(x => x.NumFromNow == MistDaysFromNow);
                MistIntervalPickerValue ??= MistingInterval.First(x => x.NumFromNow == -1);
                OnPropertyChanged(nameof(AlterPlant));
            }
            IsChangingCtoF = false;
            IsBeingAutoAdjusted = false;
            return value;
        }

        [RelayCommand]
        public async Task ValidateAlterPlantAsync()
        {
            if (UnsafePlantNames is null)
            {
                await FillUnsafePlantsAsync();
            }
            AlterPlant.Validate(UnsafePlantNames);
            OnPropertyChanged(nameof(AlterPlant));
        }

        [RelayCommand]
        protected async Task AddUpdateAsync(Plant plant)
        {
            if (IsBusy)
                return;

            try
            {
                //plant = SetPlantValues(plant);

                IsBusy = true;
                if (UnsafePlantNames is null)
                {
                    await FillUnsafePlantsAsync();
                }
                AlterPlant.Validate(UnsafePlantNames);
                if (AlterPlant.Validation.IsSuccessful)
                {
                    await _databaseService.AddUpdateNewPlantAsync(AlterPlant);
                    InitialPlant = new Plant(AlterPlant);
                    InitialWaterDaysFromNow = WaterDaysFromNow;
                    InitialMistDaysFromNow = MistDaysFromNow;
                    InitialSunDaysFromNow = SunDaysFromNow;
                    UndoAll();
                    BackText = "Done";
                    SaveText = "Update";
                } else
                {
                    await Application.Current.MainPage.DisplayAlert("OH HOLD ON!", plant.Validation.Message, "Gotcha");
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to add or update plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                if (plant.Validation.IsSuccessful)
                {
                    FriendlyLabel = _databaseService.StatusMessage;
                    ShouldGetNewData = true;
                    await FriendlyLabelToastAsync();
                }
            }
        }

        [RelayCommand]
        protected async Task SwitchDetailsAsync(bool isSetup)
        {
            if (isSetup)
            {
                var remove = Shell.Current.Navigation.PopAsync();
                await Shell.Current.GoToAsync($"{nameof(PlantDetailsPage)}", true, new Dictionary<string, object>
                {
                    {"Plant", AlterPlant },
                    {"PlantGroup", await _databaseService.GetAllPlantGroupAsync() }
                });
                await remove;
            }
            else
            {
                var remove = Shell.Current.Navigation.PopAsync();
                await Shell.Current.GoToAsync($"{nameof(PlantDetailsSetupPage)}", true, new Dictionary<string, object>
                {
                    {"Plant", AlterPlant },
                    {"PlantGroup", await _databaseService.GetAllPlantGroupAsync() }
                });
                await remove;
            }
            return;
        }

        [RelayCommand]
        protected async Task DeleteAsync(Plant plant)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                await _databaseService.DeletePlantAsync(InitialPlant);
                UndoAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to delete plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                ShouldGetNewData = true;
                await GoToTableAsync();
            }
        }

        [RelayCommand]
        public async Task GoToTableAsync()
        {
            var accept = true;
            var message = GroupPickerUnsavedChangesWarning + ImageUnsavedChangesWarning + SpeciesUnsavedChangesWarning + GivenNameUnsavedChangesWarning +
                DateOfBirthUnsavedChangesWarning + WaterIntervalUnsavedChangesWarning + LastWateredUnsavedChangesWarning + NextWateringUnsavedChangesWarning +
                MistIntervalUnsavedChangesWarning + LastMistingUnsavedChangesWarning + NextMistingUnsavedChangesWarning + MoveIntervalUnsavedChangesWarning +
                LastMovedUnsavedChangesWarning + NextMoveUnsavedChangesWarning;
            
            if (!string.IsNullOrEmpty(message))
            {
                accept = await Application.Current.MainPage.DisplayAlert("You have unsaved changes? Are you sure you want to go back?", message, "Go Back", "Stay");
            }
            if (accept)
            {
                UndoAll();
                await Shell.Current.GoToAsync($"///{nameof(TablePage)}", true, new Dictionary<string, object>
            {
                {"ShouldGetNewData", true },
                {"ShouldGetNewGroupData", true }
            });
            }
            
            return;
        }

        [RelayCommand]
        protected async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync($"..", true, new Dictionary<string, object>
            {
                {"ShouldGetNewData", ShouldGetNewData },
                {"ShouldGetNewGroupData", true }
            });
            return;
        }

        [RelayCommand]
        protected void SetToDefaultMorningTime(string timeValue)
        {
            AlterPlant.GetType().GetProperty(timeValue).SetValue(AlterPlant, DateTime.FromBinary(Preferences.Default.Get("morning_time_date", new DateTime(1, 1, 1, 8, 0, 0).ToBinary())).TimeOfDay);
            OnPropertyChanged(nameof(AlterPlant));
        }

        [RelayCommand]
        protected void SetToDefaultMiddayTime(string timeValue)
        {
            AlterPlant.GetType().GetProperty(timeValue).SetValue(AlterPlant, DateTime.FromBinary(Preferences.Default.Get("midday_time_date", new DateTime(1, 1, 1, 12, 0, 0).ToBinary())).TimeOfDay);
            OnPropertyChanged(nameof(AlterPlant));
        }

        [RelayCommand]
        protected void SetToDefaultNightTime(string timeValue)
        {
            AlterPlant.GetType().GetProperty(timeValue).SetValue(AlterPlant, DateTime.FromBinary(Preferences.Default.Get("night_time_date", new DateTime(1, 1, 1, 16, 0, 0).ToBinary())).TimeOfDay);
            OnPropertyChanged(nameof(AlterPlant));
        }

        [ObservableProperty]
        private Plant plant;


    }
}
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

        public List<AutoFillPlant> PlantSuggestions { get; set; } = new();

        [ObservableProperty]
        protected bool isBeingUndone = false;


        // I moved the _databaseService to the base viewmodel because just about every page was going to use it.
        [ObservableProperty]
        protected bool groupUndoButtonVisible = false;
        [RelayCommand]
        protected void GroupPickerChanged() { GroupUndoButtonVisible = (!IsInitialization && !IsBeingUndone) ? true : false; }
        [RelayCommand]
        protected void GroupSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.PlantGroupName = InitialPlant.PlantGroupName;
            AlterPlant.GroupColorHexString = InitialPlant.GroupColorHexString;
            GroupPickerValue = AlterPlant.PlantGroupName != null ? PlantGroups.FirstOrDefault(_ => _.GroupName == AlterPlant.PlantGroupName) : PlantGroups.FirstOrDefault(_ => _.GroupName == "Ungrouped");
            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            GroupUndoButtonVisible = false;
        }

        [ObservableProperty]
        protected bool imageUndoButtonVisible = false;
        [RelayCommand]
        protected void ImageChanged() { ImageUndoButtonVisible = (!IsInitialization && !IsBeingUndone) ? true : false; }
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

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            ImageUndoButtonVisible = false;
        }

        [ObservableProperty]
        protected bool speciesUndoButtonVisible = false;
        [RelayCommand]
        protected void SpeciesChanged() { SpeciesUndoButtonVisible = (!IsInitialization && !IsBeingUndone && AlterPlant.PlantSpecies != InitialPlant.PlantSpecies) ? true : false; }
        [RelayCommand]
        protected void SpeciesChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.PlantSpecies = InitialPlant.PlantSpecies;

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            SpeciesUndoButtonVisible = false;
        }

        [ObservableProperty]
        protected bool givenNameUndoButtonVisible = false;
        [RelayCommand]
        protected void GivenNameChanged() { GivenNameUndoButtonVisible = (!IsInitialization && !IsBeingUndone && AlterPlant.GivenName != InitialPlant.GivenName) ? true : false; }
        [RelayCommand]
        protected void GivenNameChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.GivenName = InitialPlant.GivenName;

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            GivenNameUndoButtonVisible = false;
        }

        [ObservableProperty]
        protected bool dobUndoButtonVisible = false;
        [RelayCommand]
        protected void DobChanged() { DobUndoButtonVisible = (!IsInitialization && !IsBeingUndone && AlterPlant.DateOfBirth != InitialPlant.DateOfBirth) ? true : false; }
        [RelayCommand]
        protected void DobChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.DateOfBirth = InitialPlant.DateOfBirth;

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            DobUndoButtonVisible = false;
        }

        [ObservableProperty]
        protected bool waterIntervalUndoButtonVisible = false;
        [RelayCommand]
        protected void WaterIntervalChanged() { WaterIntervalUndoButtonVisible = (!IsInitialization && !IsBeingUndone) ? true : false; }
        [RelayCommand]
        protected void WaterIntervalChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.WaterInterval = InitialPlant.WaterInterval;
            WaterDaysFromNow = AlterPlant.WaterInterval != null ? (int)AlterPlant.WaterInterval : (AlterPlant.DateOfNextWatering.Date - AlterPlant.DateOfLastWatering.Date).Days;
            WaterIntervalPickerValue = WateringInterval.FirstOrDefault(x => x.NumFromNow == InitialWaterDaysFromNow);
            if (WaterIntervalPickerValue == null)
            {
                WaterIntervalPickerValue = WateringInterval.First(x => x.NumFromNow == -1);
            }

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            WaterIntervalUndoButtonVisible = false;
        }

        [ObservableProperty]
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
        [RelayCommand]
        protected void LastWateredChangedSectionUndo()
        {
            IsBeingUndone = true;

            AlterPlant.DateOfLastWatering = InitialPlant.DateOfLastWatering;
            AlterPlant.TimeOfLastWatering = InitialPlant.TimeOfLastWatering;

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            LastWateredUndoButtonVisible = false;
        }

        [ObservableProperty]
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
        [RelayCommand]
        protected void NextWaterChangedSectionUndo()
        {
            IsBeingUndone = true;

            AlterPlant.DateOfNextWatering = InitialPlant.DateOfNextWatering;
            AlterPlant.TimeOfNextWatering = InitialPlant.TimeOfNextWatering;

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            NextWaterUndoButtonVisible = false;
        }

        [ObservableProperty]
        protected bool mistIntervalUndoButtonVisible = false;
        [RelayCommand]
        protected void MistIntervalChanged() { MistIntervalUndoButtonVisible = (!IsInitialization && !IsBeingUndone) ? true : false; }
        [RelayCommand]
        protected void MistIntervalChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.MistInterval = InitialPlant.MistInterval;
            MistDaysFromNow = AlterPlant.MistInterval != null ? (int)AlterPlant.MistInterval : (AlterPlant.DateOfNextMisting.Date - AlterPlant.DateOfLastMisting.Date).Days;
            MistIntervalPickerValue = MistingInterval.FirstOrDefault(x => x.NumFromNow == InitialMistDaysFromNow);
            if (MistIntervalPickerValue == null)
            {
                MistIntervalPickerValue = MistingInterval.First(x => x.NumFromNow == -1);
            }

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            MistIntervalUndoButtonVisible = false;
        }

        [ObservableProperty]
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
        [RelayCommand]
        protected void LastMistedChangedSectionUndo()
        {
            IsBeingUndone = true;

            AlterPlant.DateOfLastMisting = InitialPlant.DateOfLastMisting;
            AlterPlant.TimeOfLastMisting = InitialPlant.TimeOfLastMisting;

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            LastMistedUndoButtonVisible = false;
        }

        [ObservableProperty]
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
        [RelayCommand]
        protected void NextMistChangedSectionUndo()
        {
            IsBeingUndone = true;

            AlterPlant.DateOfNextMisting = InitialPlant.DateOfNextMisting;
            AlterPlant.TimeOfNextMisting = InitialPlant.TimeOfNextMisting;

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            NextMistUndoButtonVisible = false;
        }

        [ObservableProperty]
        protected bool moveIntervalUndoButtonVisible = false;
        [RelayCommand]
        protected void MoveIntervalChanged() { MoveIntervalUndoButtonVisible = (!IsInitialization && !IsBeingUndone) ? true : false; }
        [RelayCommand]
        protected void MoveIntervalChangedSectionUndo()
        {
            IsBeingUndone = true;
            AlterPlant.SunInterval = InitialPlant.SunInterval;
            SunDaysFromNow = AlterPlant.SunInterval != null ? (int)AlterPlant.SunInterval : (AlterPlant.DateOfNextMove.Date - AlterPlant.DateOfLastMove.Date).Days;
            SunIntervalPickerValue = SunInterval.FirstOrDefault(x => x.NumFromNow == InitialSunDaysFromNow);
            if (SunIntervalPickerValue == null)
            {
                SunIntervalPickerValue = SunInterval.First(x => x.NumFromNow == -1);
            }

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            MoveIntervalUndoButtonVisible = false;
        }

        [ObservableProperty]
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
        [RelayCommand]
        protected void LastMovedChangedSectionUndo()
        {
            IsBeingUndone = true;

            AlterPlant.DateOfLastMove = InitialPlant.DateOfLastMove;
            AlterPlant.TimeOfLastMove = InitialPlant.TimeOfLastMove;

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            LastMovedUndoButtonVisible = false;
        }

        [ObservableProperty]
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
        [RelayCommand]
        protected void NextMoveChangedSectionUndo()
        {
            IsBeingUndone = true;

            AlterPlant.DateOfNextMove = InitialPlant.DateOfNextMove;
            AlterPlant.TimeOfNextMove = InitialPlant.TimeOfNextMove;

            OnPropertyChanged("AlterPlant");
            IsBeingUndone = false;
            NextMoveUndoButtonVisible = false;
        }

        //Not sure if this one is ready for prime time yet :(
        [RelayCommand]
        protected void UndoAll()
        {
            IsBeingUndone = true;
            //AlterPlant = new Plant(InitialPlant); This is causing visual bugs
            OnPropertyChanged("AlterPlant");
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

        public Plant InitialPlant;

        private Random rand = new Random();

        [ObservableProperty]
        public List<GroupColors> groupColors = PickerService.GetSelectableColors();

        [ObservableProperty]
        public GroupColors selectedGroupColor;

        [ObservableProperty]
        public Plant alterPlant;

        [ObservableProperty]
        public List<ClipetSpeechBubble> speechBubbles;

        public ObservableCollection<PlantGroup> PlantGroups { get; set; } = new();

        public PlantDetailsViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            wateringInterval = PickerService.GetWaterIntervals();
            mistingInterval = PickerService.GetWaterIntervals();
            sunInterval = PickerService.GetWaterIntervals();
            SelectedGroupColor = GroupColors[rand.Next(GroupColors.Count)];
        }

        public void correctlySizeTimePickerBoxes()
        {
            AlterPlant.TimeOfLastMisting = new TimeSpan(0, 0, 0);
            AlterPlant.TimeOfLastWatering = new TimeSpan(0, 0, 0);
            AlterPlant.TimeOfLastMove = new TimeSpan(0, 0, 0);
            AlterPlant.TimeOfNextMisting = new TimeSpan(0, 0, 0);
            AlterPlant.TimeOfNextWatering = new TimeSpan(0, 0, 0);
            AlterPlant.TimeOfNextMove = new TimeSpan(0, 0, 0);

            AlterPlant.TimeOfLastMisting = InitialPlant.TimeOfLastMisting;
            AlterPlant.TimeOfLastWatering = InitialPlant.TimeOfLastWatering;
            AlterPlant.TimeOfLastMove = InitialPlant.TimeOfLastMove;
            AlterPlant.TimeOfNextMisting = InitialPlant.TimeOfNextMisting;
            AlterPlant.TimeOfNextWatering = InitialPlant.TimeOfNextWatering;
            AlterPlant.TimeOfNextMove = InitialPlant.TimeOfNextMove;
        }

        [RelayCommand]
        public void Appearing()
        {
            IsInitialization = true;
            correctlySizeTimePickerBoxes();
            // extract to its own reusable method with reflection DRY!
            GroupPickerValue = AlterPlant.PlantGroupName != null ? PlantGroups.FirstOrDefault(_ => _.GroupName == AlterPlant.PlantGroupName) : PlantGroups.FirstOrDefault(_ => _.GroupName == "Ungrouped");

            InitialWaterDaysFromNow = WaterDaysFromNow = AlterPlant.WaterInterval != null ? (int)AlterPlant.WaterInterval : (AlterPlant.DateOfNextWatering.Date - AlterPlant.DateOfLastWatering.Date).Days;
            WaterIntervalPickerValue = WateringInterval.FirstOrDefault(x => x.NumFromNow == WaterDaysFromNow);
            if (WaterIntervalPickerValue == null)
            {
                WaterIntervalPickerValue = WateringInterval.First(x => x.NumFromNow == -1);
            }

            InitialMistDaysFromNow = MistDaysFromNow = AlterPlant.MistInterval != null ? (int)AlterPlant.MistInterval : (AlterPlant.DateOfNextMisting.Date - AlterPlant.DateOfLastMisting.Date).Days;
            MistIntervalPickerValue = MistingInterval.FirstOrDefault(x => x.NumFromNow == MistDaysFromNow);
            if (MistIntervalPickerValue == null)
            {
                MistIntervalPickerValue = MistingInterval.First(x => x.NumFromNow == -1);
            }
            InitialSunDaysFromNow = SunDaysFromNow = AlterPlant.SunInterval != null ? (int)AlterPlant.SunInterval : (AlterPlant.DateOfNextMove.Date - AlterPlant.DateOfLastMove.Date).Days;
            SunIntervalPickerValue = SunInterval.FirstOrDefault(x => x.NumFromNow == SunDaysFromNow);
            if (SunIntervalPickerValue == null)
            {
                SunIntervalPickerValue = SunInterval.First(x => x.NumFromNow == -1);
            }

            WaterGridText = AlterPlant.UseWatering ? "Do Not Use Watering" : "Use Watering";
            MistGridText = AlterPlant.UseMisting ? "Do Not Use Misting" : "Use Misting";
            SunGridText = AlterPlant.UseMoving ? "Do Not Use Sunlight Move" : "Use Sunlight Move";

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

            IsInitialization = false;
        }

        public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsInitialization = true;
            AlterPlant = query["Plant"] as Plant;
            InitialPlant = new Plant(AlterPlant);
            OnPropertyChanged("AlterPlant");
            var groups = query["PlantGroup"] as List<PlantGroup>;
            PlantGroups = new ObservableCollection<PlantGroup>(groups);
            OnPropertyChanged("PlantGroups");
            IsInitialization = false;
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
                OnPropertyChanged("AlterPlant");
                SetImageSourceOfPlant();
            } 
            catch(Exception ex) 
            {
                AlterPlant.ImageLocation = AlterPlant.ImageLocation;
                OnPropertyChanged("AlterPlant");
            }
            
        }

        [RelayCommand]
        public void SetImageSourceOfPlant()
        {
            AlterPlant.PlantImageSource = Base64ImageConverterService.Base64ToImage(AlterPlant.ImageLocation);
            IsImageSelected = AlterPlant.PlantImageSource is not null ? true : false;
            ImageChanged();
            OnPropertyChanged("AlterPlant");
            OnPropertyChanged("IsImageSelected");
        }

        partial void OnGroupPickerValueChanged(PlantGroup value)
        {
            if (!IsInitialization)
            {
                AlterPlant.PlantGroupName = value.GroupName;
                AlterPlant.GroupColorHexString = value.GroupColorHex;
                OnPropertyChanged("AlterPlant");
            }
        }

        partial void OnWaterDaysFromNowChanged(double value)
        {
            if (!IsInitialization)
            {
                AlterPlant.DateOfNextWatering = AlterPlant.DateOfLastWatering.AddDays(value);
            }

            AlterPlant.WaterInterval = value;
            OnPropertyChanged("AlterPlant");
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
            if (!IsInitialization)
            {
                AlterPlant.DateOfNextMisting = AlterPlant.DateOfLastMisting.AddDays(value);
            }

            AlterPlant.MistInterval = value;
            OnPropertyChanged("AlterPlant");
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
            OnPropertyChanged("AlterPlant");
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
            var result = await AddUpdateGroupAsync(newPlantGroup);
            if (result)
            {
                PlantGroups.Add(newPlantGroup);
                OnPropertyChanged("PlantGroups");
                AddGroupShowPressed();
                GroupPickerValue = newPlantGroup;
            }
        }

        [RelayCommand]
        protected void ClearImage()
        {
            AlterPlant.ImageLocation = null;
            AlterPlant.PlantImageSource = null;
            IsImageSelected = false;
            OnPropertyChanged("AlterPlant");
        }

        [RelayCommand]
        protected void UseWateringPressed(bool value)
        {
            AlterPlant.UseWatering = !AlterPlant.UseWatering;
            WaterGridText = AlterPlant.UseWatering ? "Do Not Use Watering" : "Use Watering";
            OnPropertyChanged("AlterPlant");
        }

        [RelayCommand]
        protected void UseMistingPressed(bool value)
        {
            AlterPlant.UseMisting = !AlterPlant.UseMisting;
            MistGridText = AlterPlant.UseMisting ? "Do Not Use Misting" : "Use Misting";
            OnPropertyChanged("AlterPlant");
        }

        [RelayCommand]
        protected void UseSunPressed(bool value)
        {
            AlterPlant.UseMoving = !AlterPlant.UseMoving;
            SunGridText = AlterPlant.UseMoving ? "Do Not Use Sunlight Move" : "Use Sunlight Move";
            OnPropertyChanged("AlterPlant");
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

        [RelayCommand]
        protected async Task AddUpdateAsync(Plant plant)
        {
            if (IsBusy)
                return;

            try
            {
                //plant = SetPlantValues(plant);

                IsBusy = true;
                await _databaseService.AddUpdateNewPlantAsync(plant);
                InitialPlant = new Plant(AlterPlant);
                InitialWaterDaysFromNow = WaterDaysFromNow;
                InitialMistDaysFromNow = MistDaysFromNow;
                InitialSunDaysFromNow = SunDaysFromNow;
                UndoAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to add or update plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                FriendlyLabel = _databaseService.StatusMessage;
                ShouldGetNewData = true;
                await FriendlyLabelToastAsync();
            }
        }

        [RelayCommand]
        protected async Task SwitchDetailsAsync(bool isSetup)
        {
            if (isSetup)
            {
                await Shell.Current.GoToAsync(nameof(PlantDetailsPage), true, new Dictionary<string, object>
                {
                    {"Plant", AlterPlant },
                    {"PlantGroup", await _databaseService.GetAllPlantGroupAsync() }
                });
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(PlantDetailsSetupPage), true, new Dictionary<string, object>
                {
                    {"Plant", AlterPlant },
                    {"PlantGroup", await _databaseService.GetAllPlantGroupAsync() }
                });
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
                await _databaseService.DeletePlantAsync(plant);
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
        protected async Task GoToTableAsync()
        {
            await Shell.Current.GoToAsync($"///{nameof(TablePage)}", true, new Dictionary<string, object>
            {
                {"ShouldGetNewData", ShouldGetNewData },
                {"ShouldGetNewGroupData", ShouldGetNewGroupData }
            });
            return;
        }

        [RelayCommand]
        protected void SetToDefaultMorningTime(string timeValue)
        {
            AlterPlant.GetType().GetProperty(timeValue).SetValue(AlterPlant, DateTime.FromBinary(Preferences.Default.Get("morning_time_date", new DateTime(1, 1, 1, 8, 0, 0).ToBinary())).TimeOfDay);
            OnPropertyChanged("AlterPlant");
        }

        [RelayCommand]
        protected void SetToDefaultMiddayTime(string timeValue)
        {
            AlterPlant.GetType().GetProperty(timeValue).SetValue(AlterPlant, DateTime.FromBinary(Preferences.Default.Get("midday_time_date", new DateTime(1, 1, 1, 12, 0, 0).ToBinary())).TimeOfDay);
            OnPropertyChanged("AlterPlant");
        }

        [RelayCommand]
        protected void SetToDefaultNightTime(string timeValue)
        {
            AlterPlant.GetType().GetProperty(timeValue).SetValue(AlterPlant, DateTime.FromBinary(Preferences.Default.Get("night_time_date", new DateTime(1, 1, 1, 16, 0, 0).ToBinary())).TimeOfDay);
            OnPropertyChanged("AlterPlant");
        }

        [ObservableProperty]
        private Plant plant;


    }
}
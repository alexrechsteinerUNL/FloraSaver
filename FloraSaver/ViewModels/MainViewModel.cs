using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.ViewModels
{
    public partial class MainViewModel : TableViewModel, INotifyPropertyChanged
    {

        public ObservableCollection<ClipetDialog> Dialogs { get; set; }= new();
        [ObservableProperty]
        public ClipetDialog currentDialog;

        public readonly List<int> importantTimes = new() { 0, 1, 10, 20, 30, 40, 50, 60, 70 };

        [ObservableProperty]
        public int treatsGiven = 0;

        [ObservableProperty]
        public Plant nextPlant;

        [ObservableProperty]
        public bool isNextPlantVisible = false;

        [ObservableProperty]
        public bool isNextWater = false;

        [ObservableProperty]
        public bool isNextMist = false;

        [ObservableProperty]
        public bool isNextSun = false;

        [ObservableProperty]
        public bool isExclaimTouchClipet = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(UseFullImage))]
        public bool useStretchedImage = false;
        public bool UseFullImage => !UseStretchedImage;

        private double ImageRatio = .5625;

        public MainViewModel(IDatabaseService databaseService, IPlantNotificationService plantNotificationService) : base(databaseService, plantNotificationService)
        {
            databaseService = _databaseService;
            plantNotificationService = _plantNotificationService;
            TreatsGiven = Preferences.Default.Get("current_treat_count", 0);
            IsExclaimTouchClipet = Preferences.Default.Get("is_exclaim", true);

        }

        [RelayCommand]
        private async Task AppearingDialogsAsync()
        {
            Dialogs = new(await _databaseService.GetAllClipetDialogsAsync());
        }

        [RelayCommand]
        private async Task AppearingHomeAsync()
        {
            if (ShouldUpdateCheckService.shouldGetNewPlantDataMain) { await GetPlantsAsync(); ShouldUpdateCheckService.shouldGetNewPlantDataMain = false; }
            //await _databaseService.PopulateClipetDialogTableAsync(); //Testing
            Dialogs = new(await _databaseService.GetAllClipetDialogsAsync());
            _ = PeriodicTimerUpdaterBackgroundAsync(() => CheatUpdateAllPlantProgress());
            if (DataPlants.Count > 0)
            {
                AreNoPlants = false;
            }
            else
            {
                AreNoPlants = true;
            }
            SetNextPlant();
        }

        public void ReconfigureValuesForScreenSize(double width, double height)
        {
            if (width/height < ImageRatio)
            {
                UseStretchedImage = true;
            } else
            {
                UseStretchedImage = false;
            }
        }

        public void SetNextPlant()
        {
            NextPlant = DataPlants.OrderByDescending(_ => new[] { _.WaterPercent, _.MistPercent, _.SunPercent }.Max()).ThenByDescending(_ => new[] { _.UseWatering, _.UseMisting, _.UseMoving }.Max()).FirstOrDefault();
            if (NextPlant != null)
            {
                IsNextPlantVisible = true;
                var nextElement = new[] { NextPlant.WaterPercent, NextPlant.MistPercent, NextPlant.SunPercent }.Max();
                switch (nextElement)
                {
                    case var value when value == NextPlant.WaterPercent:
                        IsNextWater = true;
                        IsNextMist = false;
                        IsNextSun = false;
                        break;

                    case var value when value == NextPlant.MistPercent:
                        IsNextWater = false;
                        IsNextMist = true;
                        IsNextSun = false;
                        break;

                    case var value when value == NextPlant.SunPercent:
                        IsNextWater = false;
                        IsNextMist = false;
                        IsNextSun = true;
                        break;

                    default:
                        IsNextWater = false;
                        IsNextMist = false;
                        IsNextSun = false;
                        break;
                }
            }
        }

        [RelayCommand]
        public void TreatGiven()
        {
            if (TreatsGiven < 999)
            {
                TreatsGiven++;
                if (importantTimes.Contains(TreatsGiven)) 
                {
                    Preferences.Default.Set("is_exclaim", true);
                    IsExclaimTouchClipet = true;
                }
            }
                
            Preferences.Default.Set("current_treat_count", TreatsGiven);
        }

        [RelayCommand]
        public async Task SetCurrentDialogAsync()
        {
            if (IsExclaimTouchClipet) { IsExclaimTouchClipet = false; Preferences.Default.Set("is_exclaim", false); }
            Random rand = new Random();
            List<ClipetDialog> newlyUnlockedDialogs = new();
            var lockedDialogs = Dialogs.Where(_ => !_.IsUnlocked).ToList();
            if (lockedDialogs.Count > 0)
            {
                newlyUnlockedDialogs = lockedDialogs.Where(_ => _.TreatRequirement <= TreatsGiven).ToList();
                if (newlyUnlockedDialogs.Count > 0)
                {
                    var sortedDialogs = newlyUnlockedDialogs.Where(_ => _.NewlyUnlockedOrder != -1).OrderBy(_ => _.NewlyUnlockedOrder).ToList();

                    CurrentDialog = (sortedDialogs.Count > 0) ? sortedDialogs[0] :  newlyUnlockedDialogs[rand.Next(newlyUnlockedDialogs.Count)];
                    Dialogs.FirstOrDefault(_ => _ == CurrentDialog).IsUnlocked = true;
                    var updateActionNew = _databaseService.UpdateClipetDialogTableAsync(Dialogs.ToList());
                    await TalkToClipetAsync(CurrentDialog.Filename);
                    await updateActionNew;
                    return;
                }
            }

            var unlockedDialogs = Dialogs.Where(_ => _.IsUnlocked && _.TreatRequirement <= TreatsGiven).ToList();
            if (unlockedDialogs.Count > 0 && newlyUnlockedDialogs.Count <= 0)
            {
                var unseenDialogs = unlockedDialogs.Where(_ => !_.IsSeen).ToList();
                if (unseenDialogs.Count > 0)
                {
                    CurrentDialog = unseenDialogs[rand.Next(unseenDialogs.Count)];
                } 
                else
                {
                CurrentDialog = unlockedDialogs[rand.Next(unlockedDialogs.Count)];
                }
            }
            
            Dialogs.FirstOrDefault(_ => _ == CurrentDialog).IsSeen = true;

            var updateAction = _databaseService.UpdateClipetDialogTableAsync(Dialogs.ToList());
            await TalkToClipetAsync(CurrentDialog.Filename);
            await updateAction;
        }

        protected override async Task ResetWateringAsync(Plant plant)
        {
            await base.ResetWateringAsync(plant);
            SetNextPlant();
        }

        protected override async Task ResetMistingAsync(Plant plant)
        {
            await base.ResetMistingAsync(plant);
            SetNextPlant();
        }

        protected override async Task ResetMovingAsync(Plant plant)
        {
            await base.ResetMovingAsync(plant);
            SetNextPlant();
        }
    }
}
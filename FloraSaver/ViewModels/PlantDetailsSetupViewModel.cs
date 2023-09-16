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
    public partial class PlantDetailsSetupViewModel : PlantDetailsViewModel, IQueryAttributable, INotifyPropertyChanged
    {
        [ObservableProperty]
        public List<string> setupTabs = new List<string>() {"GroupName","ImageUpload", "PlantName", "GivenName", "Water", "Refresh", "Sun"};

        [ObservableProperty]
        public string activeElement;

        private Random rand = new Random();

        public PlantDetailsSetupViewModel(IDatabaseService databaseService) : base (databaseService)
        {
            _databaseService = databaseService;
            SelectedGroupColor = GroupColors[rand.Next(GroupColors.Count)];
        }

        [RelayCommand]
        void AppearingSetup()
        {
            IsInitialization = true;

            IsInitialization = false;
        }

        //public void ApplyQueryAttributes(IDictionary<string, object> query)
        //{
        //    IsInitialization = true;
        //    InitialPlant = AlterPlant = query["Plant"] as Plant;
        //    OnPropertyChanged("Plant");
        //    var groups = query["PlantGroup"] as List<PlantGroup>;
        //    PlantGroups = new ObservableCollection<PlantGroup>(groups);
        //    OnPropertyChanged("PlantGroups");
        //    IsInitialization = false;
        //}


        [ObservableProperty]
        bool isInitialization;

        [ObservableProperty]
        Plant plant;

        [ObservableProperty]
        bool groupNameDialogActive;
        [ObservableProperty]
        bool plantNameDialogActive;
        [ObservableProperty]
        bool givenNameDialogActive;
        [ObservableProperty]
        bool waterDialogActive;
        [ObservableProperty]
        bool refreshDialogActive;
        [ObservableProperty]
        bool sunDialogActive;

        [ObservableProperty]
        Color tabBackgroundGroupNameDialog = Color.FromArgb("#000000");
        [ObservableProperty]
        Color tabBackgroundPlantNameDialog = Color.FromArgb("#000000");
        [ObservableProperty]
        Color tabBackgroundGivenNameDialog = Color.FromArgb("#000000");
        [ObservableProperty]
        Color tabBackgroundWaterDialog = Color.FromArgb("#000000");
        [ObservableProperty]
        Color tabBackgroundRefreshDialog = Color.FromArgb("#000000");
        [ObservableProperty]
        Color tabBackgroundSunDialog = Color.FromArgb("#000000");

        [RelayCommand]
        public void TabPressed(string tab)
        {
            switch (tab)
            {
                case "GroupName":
                    DisableAll();
                    GroupNameDialogActive = true;
                    TabBackgroundGroupNameDialog = Color.FromArgb("#e1ad01");
                    break;
                case "PlantName":
                    DisableAll();
                    PlantNameDialogActive = true;
                    TabBackgroundPlantNameDialog = Color.FromArgb("#e1ad01");
                    break;
                case "GivenName":
                    DisableAll();
                    GivenNameDialogActive = true;
                    TabBackgroundGivenNameDialog = Color.FromArgb("#e1ad01");
                    break;
                case "Water":
                    DisableAll();
                    WaterDialogActive = true;
                    TabBackgroundWaterDialog = Color.FromArgb("#e1ad01");
                    break;
                case "Refresh":
                    DisableAll();
                    RefreshDialogActive = true;
                    TabBackgroundRefreshDialog = Color.FromArgb("#e1ad01");
                    break;
                case "Sun":
                    DisableAll();
                    SunDialogActive = true;
                    TabBackgroundSunDialog = Color.FromArgb("#e1ad01");
                    break;
                default:
                    DisableAll();
                    GroupNameDialogActive = true;
                    TabBackgroundGroupNameDialog = Color.FromArgb("#e1ad01");
                    tab = "GroupName";
                    break;
            }
            ActiveElement = tab;
        }

        [RelayCommand]
        public void NextButtonPressed()
        {
            var currentIndex = SetupTabs.IndexOf(ActiveElement);
            if (currentIndex > -1 && currentIndex < SetupTabs.Count)
            {
                TabPressed(SetupTabs[currentIndex + 1]);
            }
        }

        [RelayCommand]
        public void LastButtonPressed()
        {
            var currentIndex = SetupTabs.IndexOf(ActiveElement);
            if (currentIndex > -1 && currentIndex > 0)
            {
                TabPressed(SetupTabs[currentIndex - 1]);
            }
        }

        [RelayCommand]
        public void DisableAll()
        {
            GroupNameDialogActive = false;
            PlantNameDialogActive = false;
            GivenNameDialogActive = false;
            WaterDialogActive = false;
            RefreshDialogActive = false;
            SunDialogActive = false;
            TabBackgroundSunDialog = Color.FromArgb("#000000");
            TabBackgroundRefreshDialog = Color.FromArgb("#000000");
            TabBackgroundWaterDialog = Color.FromArgb("#000000");
            TabBackgroundGivenNameDialog = Color.FromArgb("#000000");
            TabBackgroundPlantNameDialog = Color.FromArgb("#000000");
            TabBackgroundGroupNameDialog = Color.FromArgb("#000000");
        }
    }
}

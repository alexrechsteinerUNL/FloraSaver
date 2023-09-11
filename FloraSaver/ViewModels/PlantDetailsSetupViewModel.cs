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
        bool imageUploadDialogActive;
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

        [RelayCommand]
        public void TabPressed(string tab)
        {
            switch (tab)
            {
                case "GroupName":
                    DisableAll();
                    GroupNameDialogActive = true;
                    
                    break;
                case "ImageUpload":
                    DisableAll();
                    ImageUploadDialogActive = true;
                    break;
                case "PlantName":
                    DisableAll();
                    PlantNameDialogActive = true;
                    break;
                case "GivenName":
                    DisableAll();
                    GivenNameDialogActive = true;
                    break;
                case "Water":
                    DisableAll();
                    WaterDialogActive = true;
                    break;
                case "Refresh":
                    DisableAll();
                    RefreshDialogActive = true;
                    break;
                case "Sun":
                    DisableAll();
                    SunDialogActive = true;
                    break;
                default:
                    DisableAll();
                    GroupNameDialogActive = true;
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
            ImageUploadDialogActive = false;
            PlantNameDialogActive = false;
            GivenNameDialogActive = false;
            WaterDialogActive = false;
            RefreshDialogActive = false;
            SunDialogActive = false;
        }
    }
}

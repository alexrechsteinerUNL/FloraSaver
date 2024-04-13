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
    [QueryProperty("SelectedTab", "SelectedTab")]
    public partial class PlantDetailsSetupViewModel : PlantDetailsViewModel, IQueryAttributable, INotifyPropertyChanged
    {
        public List<string> SetupTabs { get; set; } = new List<string>() { "PlantName", "GroupName", "GivenName", "Water", "Refresh", "Sun" };

        [ObservableProperty]
        public DataTab activeTab;

        [ObservableProperty]
        public string activeElement;

        private Random rand = new Random();

        public PlantDetailsSetupViewModel(IDatabaseService databaseService) : base(databaseService)
        {
            _databaseService = databaseService;
            wateringInterval = PickerService.GetWaterIntervals();
            mistingInterval = PickerService.GetWaterIntervals();
            sunInterval = PickerService.GetWaterIntervals();
            SelectedGroupColor = GroupColors[rand.Next(GroupColors.Count)];
        }

        public override void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            base.ApplyQueryAttributes(query);
            var startTab = query.ContainsKey("SelectedTab") ? SetupTabs.FirstOrDefault(_ => _ == (string)query["SelectedTab"]) ?? "PlantName" : "PlantName";
            TabPressed(startTab);
        }

        [RelayCommand]
        private async Task AppearingSetupAsync()
        {
            IsInitialization = true;
            await AppearingAsync();
            IsInitialization = false;
        }

        //public void ApplyQueryAttributes(IDictionary<string, object> query)
        //{
        //    IsInitialization = true;
        //    InitialPlant = AlterPlant = query["Plant"] as Plant;
        //    OnPropertyChanged("Plant");
        //    var groups = query["PlantGroup"] as List<PlantGroup>;
        //    PlantGroups = new ObservableCollection<PlantGroup>(groups);
        //    OnPropertyChanged(nameof(PlantGroups));
        //    IsInitialization = false;
        //}

        [ObservableProperty]
        private bool isInitialization;

        [ObservableProperty]
        private Plant plant;

        [RelayCommand]
        public void TabPressed(string tab)
        {
            switch (tab)
            {
                case "GroupName":

                    DisableAll();
                    ActiveTab = new DataTab(
                tabName: "GroupName",
                clipetText: "Add this plant to a group!",
                isActive: true
                );
                    break;

                case "PlantName":
                    DisableAll();
                    ActiveTab = new DataTab(
                tabName: "PlantName",
                clipetText: "What is the name of this plant?",
                isActive: true
                );
                    break;

                case "GivenName":
                    DisableAll();
                    ActiveTab = new DataTab(
                tabName: "GivenName",
                clipetText: "Add a personal touch!",
                isActive: true
                );
                    break;

                case "Water":
                    DisableAll();
                    ActiveTab = new DataTab(
                        tabName: "Water",
                        clipetText: "Watering?",
                        isActive: true
                        );
                    break;

                case "Refresh":
                    DisableAll();
                    ActiveTab = new DataTab(
                        tabName: "Refresh",
                        clipetText: "Refreshing?",
                        isActive: true
                        );
                    break;

                case "Sun":
                    DisableAll();
                    ActiveTab = new DataTab(
                        tabName: "Sun",
                        clipetText: "Moving?",
                        isActive: true
                        );
                    break;

                default:
                    DisableAll();
                    ActiveTab = new DataTab(
                tabName: "GroupName",
                clipetText: "Add this plant to a group!",
                isActive: true
                );
                    tab = "GroupName";
                    break;
            }
            OnPropertyChanged("ActiveTab");
            ActiveElement = tab;
        }

        [RelayCommand]
        public void NextButtonPressed()
        {
            var currentIndex = SetupTabs.IndexOf(ActiveElement);
            if (currentIndex > -1 && currentIndex < SetupTabs.Count - 1)
            {
                TabPressed(SetupTabs[currentIndex + 1]);
            }
        }

        [RelayCommand]
        public void LastButtonPressed()
        {
            var currentIndex = SetupTabs.IndexOf(ActiveElement);
            if (currentIndex > 0)
            {
                TabPressed(SetupTabs[currentIndex - 1]);
            }
        }

        [RelayCommand]
        public void DisableAll()
        {
            ActiveTab = new DataTab("", "");
        }
    }
}
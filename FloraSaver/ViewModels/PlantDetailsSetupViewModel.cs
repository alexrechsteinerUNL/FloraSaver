﻿using System;
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
    }
}

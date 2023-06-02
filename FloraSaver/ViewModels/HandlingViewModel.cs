using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;

namespace FloraSaver.ViewModels
{
    public partial class HandlingViewModel : TableViewModel, INotifyPropertyChanged
    {
        public HandlingViewModel(PlantService plantService) : base (plantService)
        {
            this.plantService = plantService;
        }

        [RelayCommand]
        void PlantSelection(Plant plant)
        {
            var specificPlant = Plants.FirstOrDefault(_ => _.Id == plant.Id);
            specificPlant.IsEnabled = plant.IsEnabled ? false : true;
            OnPropertyChanged("Plants");
        }

        [RelayCommand]
        void AllPlantSelection()
        {
            foreach (var plant in Plants)
            {
                PlantSelection(plant);
            }
        }
    }
}

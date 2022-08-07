using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;

namespace FloraSaver.ViewModels
{
    public partial class PlantsViewModel : BaseViewModel
    {
        public ObservableCollection<Plant> Plants { get; } = new();
        PlantService plantService;
        public PlantsViewModel(PlantService plantService)
        {
            Title = "Plant Finder";
            this.plantService = plantService;
        }
        
        [RelayCommand]
        async Task GetPlantsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var plants = await plantService.GetAllPlantAsync();

                if (Plants.Count != 0)
                    Plants.Clear();

                foreach (var plant in plants)
                    Plants.Add(plant);

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

        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;
using Microsoft.VisualBasic;

namespace FloraSaver.ViewModels
{
    public partial class TableViewModel : BaseViewModel
    {
        public List<Plant> DataPlants { get; } = new();
        public ObservableCollection<Plant> Plants { get; } = new();
        PlantService plantService;
        public TableViewModel(PlantService plantService)
        {
            Title = "Plant Saver";
            this.plantService = plantService;
        }

        [ObservableProperty]
        bool isRefreshing;

        [RelayCommand]
        async Task SearchPlantsAsync(string inputString)
        {
            try
            {
                await GetPlantsAsync();
                for (int i = Plants.Count - 1; i >= 0; i--)
                {
                    if (Plants[i].GivenName.IndexOf(inputString) < 0)
                    {
                        Plants.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to search plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
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
                {
                    Plants.Clear();
                }
                foreach (var plant in plants)
                {
                    Plants.Add(plant);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        async Task GoToDetailsAsync(Plant plant)
        {
            if (plant == null)
                await Shell.Current.GoToAsync(nameof(PlantDetailsPage), true, new Dictionary<string, object>
                {
                    {"Plant", new Plant { 
                        DateOfBirth = DateTime.Now, 
                        DateOfLastWatering = DateTime.Now, 
                        DateOfNextWatering = DateTime.Now } }
                });
            else
            {
                await Shell.Current.GoToAsync(nameof(PlantDetailsPage), true, new Dictionary<string, object>
                {
                    {"Plant", plant }
                });
            }
        }
    }
}

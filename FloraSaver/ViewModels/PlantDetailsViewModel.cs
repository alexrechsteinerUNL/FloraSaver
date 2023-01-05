using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;
using Microsoft.VisualStudio.RpcContracts.Commands;

namespace FloraSaver.ViewModels
{


    [QueryProperty(nameof(Plant), "Plant")]
    public partial class PlantDetailsViewModel : BaseViewModel, IQueryAttributable
    {
        PlantService plantService;

        public Plant InitialPlant { get; set; }


        // I think the code below is equivalent to
        // [ObservableProperty]
        // string friendlyLabel;
        string friendlyLabel;
        public string FriendlyLabel {
            get => friendlyLabel;
            set
            {
                friendlyLabel = value;
                OnPropertyChanged();
            }
        }

        [ObservableProperty]
        public bool customWaterInteravlGridVisible = false;

        [ObservableProperty]
        public bool showRefreshing = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CustomWaterInteravlGridVisible))]
        private string waterIntervalPickerText;

        partial void OnWaterIntervalPickerTextChanged(string value)
        {
            if (value == "7")
            {
                customWaterInteravlGridVisible = true;
            }
            else
            {
                customWaterInteravlGridVisible = false;
            }
        }


        

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            InitialPlant = query["Plant"] as Plant;
            OnPropertyChanged("Plant");
        }

        

        public PlantDetailsViewModel(PlantService plantService)
        {
            this.plantService = plantService;
        }

        [RelayCommand]
        public void UseRefreshingPressed(bool refreshStatus)
        {
            showRefreshing = refreshStatus ? false : true;
        }

        [RelayCommand]
        async Task AddUpdateAsync(Plant plant)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                await plantService.AddUpdateNewPlantAsync(plant);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to add or update plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                FriendlyLabel = plantService.StatusMessage;
            }
        }

        [RelayCommand]
        async Task DeleteAsync(Plant plant)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                await plantService.DeletePlantAsync(plant);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to add or update plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task GoToTableAsync()
        {
            await Shell.Current.GoToAsync("..");
        }



        [ObservableProperty]
        Plant plant;
    }
}

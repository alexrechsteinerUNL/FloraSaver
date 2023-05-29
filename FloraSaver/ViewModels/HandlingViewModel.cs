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
    }
}

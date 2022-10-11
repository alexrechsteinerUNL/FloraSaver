using FloraSaver.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.ViewModels
{
    internal class SettingsViewModel : BaseViewModel
    {
        PlantService plantService;
        public SettingsViewModel(PlantService plantService)
        {
            this.plantService = plantService;
        }
    }
}

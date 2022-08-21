using CommunityToolkit.Mvvm.ComponentModel;
using FloraSaver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.ViewModels
{
    [QueryProperty(nameof(Plant), "Plant")]
    public partial class PlantDetailsViewModel : BaseViewModel
    {
        public PlantDetailsViewModel()
        {
        }

        [ObservableProperty]
        Plant plant;
    }
}

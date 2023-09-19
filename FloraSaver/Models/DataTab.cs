using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Models
{
    public partial class DataTab : ObservableObject
    {
        public DataTab(string tabName, string clipetText, bool isActive = false) 
        {
            TabName = tabName;
            ClipetText = clipetText;
            IsActive = isActive;

        }
        public string TabName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public string ClipetText { get; set; } = string.Empty;
        public Color TabBackground => IsActive ? Color.FromArgb("#e1ad01") : Color.FromArgb("#000000");
    }
}

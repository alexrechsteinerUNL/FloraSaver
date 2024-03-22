using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Models
{
    public class ClipetDialog
    {
        public string Filename { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public bool IsTutorial { get; set; } = true;
        public bool IsUnlocked { get; set; } = true;
        public Color TypeColor => IsUnlocked ? (IsTutorial ? Color.FromArgb("#FF6A00") : Color.FromArgb("#4A0023")) : Color.FromArgb("#555453"); 
        public int TreatRequirement { get; set; } = 0;
    }
}

using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Models
{
    public class ClipetDialog
    {
        [PrimaryKey]
        public int DialogID { get; set; }
        public string Filename { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public bool IsTutorial { get; set; } = true;
        public bool IsSeen { get; set; } = false;
        public bool IsUnlocked { get; set; } = true;
        public int TreatRequirement { get; set; } = 0;
        public int NewlyUnlockedOrder { get; set; } = -1;
        
        [Ignore]
        public Color TypeColor => IsUnlocked ? (IsTutorial ? Color.FromArgb("#FF6A00") : Color.FromArgb("#4A0023")) : Color.FromArgb("#555453");

        
    }
}

using FloraSaver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Utilities
{
    public static class ClipetDialogGeneratorUtility
    {
        public static int ManualClipetDialogs = 4;
        public static List<ClipetDialog> AllClipetDialogs { get; set; }
        public static void GenerateClipetDialogs()
        {
            List<ClipetDialog> dialogs = new();
            dialogs.Add(new ClipetDialog() { DialogID = 0, DisplayName = "Welcome Message", Filename = "WelcomeMessage0.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 1, DisplayName = "Keep Clicking", Filename = "JustKeepClicking1.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 2, DisplayName = "Automatic Watering", Filename = "TUTAutomaticWatering.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 3, DisplayName = "Copying Plants", Filename = "TUTCopyingPlants.txt" });
            
            AllClipetDialogs = dialogs;
        }
    }
}

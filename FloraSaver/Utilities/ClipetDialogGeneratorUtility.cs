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
        public static int ManualClipetDialogs = 8;
        public static List<ClipetDialog> AllClipetDialogs { get; set; }
        public static void GenerateClipetDialogs()
        {
            List<ClipetDialog> dialogs = new();
            dialogs.Add(new ClipetDialog() { DialogID = 0, DisplayName = "Welcome Message", Filename = "WelcomeMessage0.txt", TreatRequirement = 0 });
            dialogs.Add(new ClipetDialog() { DialogID = 1, DisplayName = "Keep Clicking", Filename = "JustKeepClicking1.txt", TreatRequirement = 1 });
            dialogs.Add(new ClipetDialog() { DialogID = 2, DisplayName = "Why the T-shirt?", Filename = "TShirt2.txt", TreatRequirement = 10 });
            dialogs.Add(new ClipetDialog() { DialogID = 3, DisplayName = "How to Say Clipet", Filename = "HowToSayClipet3.txt", TreatRequirement = 20 });

            dialogs.Add(new ClipetDialog() { DialogID = 4, DisplayName = "Automatic Watering!", Filename = "TUTAutomaticWatering.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 5, DisplayName = "Copying Plants", Filename = "TUTCopyingPlants.txt"});
            dialogs.Add(new ClipetDialog() { DialogID = 6, DisplayName = "Grow Lights", Filename = "TUTGrowLights.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 7, DisplayName = "Table Page Tutorial", Filename = "TUTTablePage.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 8, DisplayName = "Handling Page", Filename = "TUTHandlingPage.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 9, DisplayName = "Plant Details Page", Filename = "TUTPlantDetails.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 10, DisplayName = "Lighting Colors JUST THE IMPORTANT PART", Filename = "TUTLightingColorsCompact.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 11, DisplayName = "Lighting Colors FULL", Filename = "TUTLightingColorsFull.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 12, DisplayName = "Leaves Turn Brown", Filename = "TUTPlantTurnsBrown.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 13, DisplayName = "What To Do With The Move Option?", Filename = "TUTThirdOption.txt" });
            AllClipetDialogs = dialogs;
        }
    }
}

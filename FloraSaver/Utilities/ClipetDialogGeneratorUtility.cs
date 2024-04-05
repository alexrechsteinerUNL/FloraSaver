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
        public static int ManualClipetDialogs = 25;
        public static List<ClipetDialog> AllClipetDialogs { get; set; }
        public static void GenerateClipetDialogs()
        {
            List<ClipetDialog> dialogs = new();
            dialogs.Add(new ClipetDialog() { DialogID = 0, DisplayName = "Welcome Message", Filename = "WelcomeMessage0.txt", IsUnlocked=false, TreatRequirement = 0, NewlyUnlockedOrder = 0 });
            dialogs.Add(new ClipetDialog() { DialogID = 1, DisplayName = "Keep Clicking", Filename = "JustKeepClicking1.txt", IsUnlocked = false, TreatRequirement = 0, NewlyUnlockedOrder = 1 });
            dialogs.Add(new ClipetDialog() { DialogID = 2, DisplayName = "Table Page Tutorial", Filename = "TUTTablePage.txt", IsUnlocked = false, TreatRequirement = 0, NewlyUnlockedOrder = 2 });
            dialogs.Add(new ClipetDialog() { DialogID = 3, DisplayName = "Plant Details Page", Filename = "TUTPlantDetails.txt", IsUnlocked = false, TreatRequirement = 0, NewlyUnlockedOrder = 3 });
            dialogs.Add(new ClipetDialog() { DialogID = 4, DisplayName = "Handling Page Tutorial", Filename = "TUTHandlingPage.txt", IsUnlocked = false, TreatRequirement = 0, NewlyUnlockedOrder = 4 });
            dialogs.Add(new ClipetDialog() { DialogID = 5, DisplayName = "Why the T-shirt?", Filename = "TShirt2.txt", IsUnlocked = false, TreatRequirement = 10, NewlyUnlockedOrder = 5 });
            dialogs.Add(new ClipetDialog() { DialogID = 6, DisplayName = "How to Say Clipet", Filename = "HowToSayClipet3.txt", IsUnlocked = false, TreatRequirement = 20, NewlyUnlockedOrder = 6 });
            dialogs.Add(new ClipetDialog() { DialogID = 7, DisplayName = "Fake Plants", Filename = "FakePlants4.txt", IsUnlocked = false, TreatRequirement = 30, NewlyUnlockedOrder = 7 });
            dialogs.Add(new ClipetDialog() { DialogID = 8, DisplayName = "Cooking", Filename = "Cooking5.txt", IsUnlocked = false, TreatRequirement = 40, NewlyUnlockedOrder = 8 });
            dialogs.Add(new ClipetDialog() { DialogID = 9, DisplayName = "Drainage", Filename = "Drainage6.txt", IsUnlocked = false, TreatRequirement = 50, NewlyUnlockedOrder = 9 });
            dialogs.Add(new ClipetDialog() { DialogID = 10, DisplayName = "Lawns", Filename = "Lawns7.txt", IsUnlocked = false, TreatRequirement = 60, NewlyUnlockedOrder = 10 });
            dialogs.Add(new ClipetDialog() { DialogID = 11, DisplayName = "Last Dialog", Filename = "LastDialog8.txt", IsUnlocked = false, TreatRequirement = 70, NewlyUnlockedOrder = 11 });


            dialogs.Add(new ClipetDialog() { DialogID = 12, DisplayName = "Automatic Watering!", Filename = "TUTAutomaticWatering.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 13, DisplayName = "Backing Up Plants", Filename = "TUTBackupPlants.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 14, DisplayName = "Copying Plants", Filename = "TUTCopyingPlants.txt"});
            dialogs.Add(new ClipetDialog() { DialogID = 15, DisplayName = "Group Edits", Filename = "TUTGroupEdits.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 16, DisplayName = "Grouping Plants", Filename = "TUTGroupingPlants.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 17, DisplayName = "Grow Lights", Filename = "TUTGrowLights.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 18, DisplayName = "Lighting Colors JUST THE IMPORTANT PART", Filename = "TUTLightingColorsCompact.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 19, DisplayName = "Lighting Colors FULL", Filename = "TUTLightingColorsFull.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 20, DisplayName = "Newly Planted Plants", Filename = "TUTNewlyPlanted.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 21, DisplayName = "Leaves Turn Brown", Filename = "TUTPlantTurnsBrown.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 22, DisplayName = "Searching", Filename = "TUTSearching.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 23, DisplayName = "Soil Moisture", Filename = "TUTSoilMoisture.txt" });
            dialogs.Add(new ClipetDialog() { DialogID = 24, DisplayName = "What To Do With The Move Option?", Filename = "TUTThirdOption.txt" });
            AllClipetDialogs = dialogs;
        }
    }
}

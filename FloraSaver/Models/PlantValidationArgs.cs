using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Models
{
    public class PlantValidationArgs
    {
        public string Message { get; set; } = "";

        public bool IsNameSectionInvalid { get; set; } = false;
        public bool IsGroupSectionInvalid { get; set; } = false;
        public bool IsSpeciesSectionInvalid { get; set; } = false;
        public bool IsBirthdaySectionInvalid { get; set; } = false;
        public bool IsDateTimeNextWateringSectionInvalid { get; set; } = false;

        private bool _isDateTimeNextMistingSectionInvalid = false;
        public bool IsDateTimeNextMistingSectionInvalid
        {
            get { return _isDateTimeNextMistingSectionInvalid; }
            set
            {
                if (value)
                {
                    Message += "Next Misting time cannot be before Last Misting Time";
                }
                _isDateTimeNextMistingSectionInvalid = value;
            }
        }

        private bool _isDateTimeNextMovingSectionInvalid = false;
        public bool IsDateTimeNextMovingSectionInvalid { get { return _isDateTimeNextMovingSectionInvalid; }
            set {
            if (value)
                {
                    Message += "Next moving time cannot be before Last Moving Time";
                }
                _isDateTimeNextMovingSectionInvalid = value;
            } 
        }

        public bool IsSuccessful {  get; set; } = false;

        public PlantValidationArgs(Plant plant)
        {
            IsSpeciesSectionInvalid = string.IsNullOrEmpty(plant.PlantSpecies);
            IsNameSectionInvalid = string.IsNullOrEmpty(plant.PlantGroupName);
            IsGroupSectionInvalid = (string.IsNullOrEmpty(plant.PlantGroupName) || string.IsNullOrEmpty(plant.GroupColorHexString);
            IsBirthdaySectionInvalid = CheckIfLastPlantDateIsFurtherThanPlantNextDate(plant.DateOfBirth, DateTime.Now);
            IsDateTimeNextWateringSectionInvalid = CheckIfLastPlantDateIsFurtherThanPlantNextDate(plant.DateOfLastWatering, plant.DateOfNextWatering);
            IsDateTimeNextMistingSectionInvalid = CheckIfLastPlantDateIsFurtherThanPlantNextDate(plant.DateOfLastMisting, plant.DateOfNextMisting);
            IsDateTimeNextMovingSectionInvalid = CheckIfLastPlantDateIsFurtherThanPlantNextDate(plant.DateOfLastMove, plant.DateOfNextMove);

        }

        public bool CheckIfLastPlantDateIsFurtherThanPlantNextDate(DateTime lastDate, DateTime nextDate)
        {
            if (lastDate > nextDate) { return true; } else { return false; }
        }


    }
}
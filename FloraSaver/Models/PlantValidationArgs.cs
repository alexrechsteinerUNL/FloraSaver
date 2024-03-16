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

        private bool _isNameSectionInvalid = false;
        public bool IsNameSectionInvalid
        {
            get { return _isNameSectionInvalid; }
            set
            {
                if (value)
                {
                    Message += "Plant Name cannot be empty or the same as another plant!\n";
                }
                _isNameSectionInvalid = value;
            }
        }
        public Color NameSectionColor => (IsNameSectionInvalid) ? Colors.OrangeRed : Colors.White;

        private bool _isGroupSectionInvalid = false;
        public bool IsGroupSectionInvalid
        {
            get { return _isGroupSectionInvalid; }
            set
            {
                if (value)
                {
                    Message += "Plant Group must exist and have a color!\n";
                }
                _isGroupSectionInvalid = value;
            }
        }
        public Color GroupSectionColor => (IsGroupSectionInvalid) ? Colors.OrangeRed : Colors.White;


        private bool _isSpeciesSectionInvalid = false;
        public bool IsSpeciesSectionInvalid
        {
            get { return _isSpeciesSectionInvalid; }
            set
            {
                if (value)
                {
                    Message += "Plant Species cannot be empty\n";
                }
                _isSpeciesSectionInvalid = value;
            }
        }
        public Color SpeciesSectionColor => (IsSpeciesSectionInvalid) ? Colors.OrangeRed : Colors.White;

        private bool _isBirthdaySectionInvalid = false;
        public bool IsBirthdaySectionInvalid
        {
            get { return _isBirthdaySectionInvalid; }
            set
            {
                if (value)
                {
                    Message += "Birthday cannot be in the future!\n";
                }
                _isBirthdaySectionInvalid = value;
            }
        }
        public Color BirthdaySectionColor => (IsBirthdaySectionInvalid) ? Colors.OrangeRed : Colors.White;

        private bool _isDateTimeNextWateringSectionInvalid = false;
        public bool IsDateTimeNextWateringSectionInvalid
        {
            get { return _isDateTimeNextWateringSectionInvalid; }
            set
            {
                if (value)
                {
                    Message += "Next Watering time cannot be before Last Misting Time\n";
                }
                _isDateTimeNextWateringSectionInvalid = value;
            }
        }
        public Color DateTimeNextWateringSectionColor => (IsDateTimeNextWateringSectionInvalid) ? Colors.OrangeRed : Colors.White;

        private bool _isDateTimeNextMistingSectionInvalid = false;
        public bool IsDateTimeNextMistingSectionInvalid
        {
            get { return _isDateTimeNextMistingSectionInvalid; }
            set
            {
                if (value)
                {
                    Message += "Next Misting time cannot be before Last Misting Time\n";
                }
                _isDateTimeNextMistingSectionInvalid = value;
            }
        }
        public Color DateTimeNextMistingSectionColor => (IsDateTimeNextMistingSectionInvalid) ? Colors.OrangeRed : Colors.White;

        private bool _isDateTimeNextMovingSectionInvalid = false;
        public bool IsDateTimeNextMovingSectionInvalid { get { return _isDateTimeNextMovingSectionInvalid; }
            set {
            if (value)
                {
                    Message += "Next Moving time cannot be before Last Moving Time\n";
                }
                _isDateTimeNextMovingSectionInvalid = value;
            } 
        }
        public Color DateTimeNextMovingSectionColor => (IsDateTimeNextMovingSectionInvalid) ? Colors.OrangeRed : Colors.White;

        public bool IsSuccessful => string.IsNullOrEmpty(Message);

        public void Validate(Plant plant, List<string> unsafePlantNames)
        {
            Message = string.Empty;
            IsSpeciesSectionInvalid = string.IsNullOrEmpty(plant.PlantSpecies);
            IsNameSectionInvalid = string.IsNullOrEmpty(plant.GivenName) || unsafePlantNames.Contains(plant.GivenName);
            IsGroupSectionInvalid = (string.IsNullOrEmpty(plant.PlantGroupName) || string.IsNullOrEmpty(plant.GroupColorHexString));
            IsBirthdaySectionInvalid = CheckIfLastDateFurtherThanNextDate(plant.DateOfBirth, DateTime.Now);
            IsDateTimeNextWateringSectionInvalid = CheckIfLastDateFurtherThanNextDate(plant.DateOfLastWatering + plant.TimeOfLastWatering, plant.DateOfNextWatering + plant.TimeOfNextWatering);
            IsDateTimeNextMistingSectionInvalid = CheckIfLastDateFurtherThanNextDate(plant.DateOfLastMisting + plant.TimeOfLastMisting, plant.DateOfNextMisting + plant.TimeOfNextMisting);
            IsDateTimeNextMovingSectionInvalid = CheckIfLastDateFurtherThanNextDate(plant.DateOfLastMove + plant.TimeOfLastMove, plant.DateOfNextMove + plant.TimeOfNextMove);
        }

        public bool CheckIfLastDateFurtherThanNextDate(DateTime lastDate, DateTime nextDate)
        {
            if (lastDate > nextDate) { return true; } else { return false; }
        }


    }
}
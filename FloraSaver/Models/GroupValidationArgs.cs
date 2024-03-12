using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Models
{
    public class GroupValidationArgs
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
                    Message += "Group Name cannot be empty or the same as another plant!\n";
                }
                _isNameSectionInvalid = value;
            }
        }
        public Color NameSectionColor => (IsNameSectionInvalid) ? Colors.OrangeRed : Colors.White;

        private bool _isGroupSectionInvalid = false;
        public bool IsColorSectionInvalid
        {
            get { return _isGroupSectionInvalid; }
            set
            {
                if (value)
                {
                    Message += "Plant Group Color must exist!\n";
                }
                _isGroupSectionInvalid = value;
            }
        }
        public Color ColorSectionColor => (IsColorSectionInvalid) ? Colors.OrangeRed : Colors.White;

        public bool IsSuccessful => string.IsNullOrEmpty(Message);

        public void Validate(PlantGroup plantGroup, List<string> unsafeGroupNames)
        {
            Message = string.Empty;
            IsColorSectionInvalid = string.IsNullOrEmpty(plantGroup.GroupColorHex);
            IsNameSectionInvalid = string.IsNullOrEmpty(plantGroup.GroupName) || unsafeGroupNames.Contains(plantGroup.GroupName);
            
        }


    }
}
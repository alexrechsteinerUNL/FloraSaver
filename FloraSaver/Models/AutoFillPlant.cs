using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Models
{
    public class AutoFillPlant : Plant
    {
        string PlantSource { get; set; }

        //maybe don't just do the base and instead use predefinied intervals to construct the plants when they are found from the database.
        public AutoFillPlant(IPlant _Plant, string source = null) : base(_Plant)
        {
            if (!string.IsNullOrEmpty(source))
            {
                PlantSource = source;
            }
        }

        public AutoFillPlant() : base()
        {
        }
    }
}

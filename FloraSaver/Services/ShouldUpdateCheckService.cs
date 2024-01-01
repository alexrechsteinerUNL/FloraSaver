using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Services
{
    public static class ShouldUpdateCheckService
    {
        public static bool shouldGetNewPlantDataMain { get; set; } = true;
        public static bool shouldGetNewPlantDataTable { get; set; } = true;
        public static bool shouldGetNewPlantDataHandling { get; set; } = true;
        public static bool shouldGetNewGroupDataTable { get; set; } = true;
        public static bool shouldGetNewGroupDataHandling { get; set; } = true;
        public static bool shouldGetNewGroupDataSettings { get; set; } = true;

        public static void ForceToGetNewPlantData()
        {
            shouldGetNewPlantDataMain = true;
            shouldGetNewPlantDataTable = true;
            shouldGetNewPlantDataHandling = true;
        }

        public static void ForceToGetNewGroupData()
        {

            shouldGetNewGroupDataTable = true;
            shouldGetNewGroupDataHandling = true;
            shouldGetNewGroupDataSettings = true;
        }
    }
}

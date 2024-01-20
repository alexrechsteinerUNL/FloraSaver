namespace FloraSaver.Models
{
    public interface IPlant
    {
        int Id { get; set; }
        string PlantSpecies { get; set; }
        public double? WaterInterval { get; set; }
        double? MistInterval { get; set; }
        double? SunInterval { get; set; }

    }
}
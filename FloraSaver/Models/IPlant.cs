namespace FloraSaver.Models
{
    public interface IPlant
    {
        DateTime DateOfBirth { get; set; }
        DateTime DateOfLastMisting { get; set; }
        DateTime DateOfLastMove { get; set; }
        DateTime DateOfLastWatering { get; set; }
        DateTime DateOfNextMisting { get; set; }
        DateTime DateOfNextMove { get; set; }
        DateTime DateOfNextWatering { get; set; }
        string GivenName { get; set; }
        int Id { get; set; }
        string ImageLocation { get; set; }
        int? MistInterval { get; set; }
        string PlantSpecies { get; set; }
        int? SunInterval { get; set; }
        TimeSpan TimeOfLastMisting { get; set; }
        TimeSpan TimeOfLastMove { get; set; }
        TimeSpan TimeOfLastWatering { get; set; }
        TimeSpan TimeOfNextMisting { get; set; }
        TimeSpan TimeOfNextMove { get; set; }
        TimeSpan TimeOfNextWatering { get; set; }
        public int? WaterInterval { get; set; }

        double TimeToNextAction(DateTime lastTime, DateTime nextTime);
    }
}
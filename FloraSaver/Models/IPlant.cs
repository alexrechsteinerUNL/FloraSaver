namespace FloraSaver.Models
{
    public interface IPlant
    {
        DateTime DateOfBirth { get; set; }
        DateTime DateOfLastMisting { get; set; }
        DateTime DateOfLastSunMove { get; set; }
        DateTime DateOfLastWatering { get; set; }
        DateTime DateOfNextMisting { get; set; }
        DateTime DateOfNextSunMove { get; set; }
        DateTime DateOfNextWatering { get; set; }
        string GivenName { get; set; }
        int Id { get; set; }
        string ImageLocation { get; set; }
        int? MistInterval { get; set; }
        string PlantSpecies { get; set; }
        int? SunInterval { get; set; }
        TimeSpan TimeOfLastMisting { get; set; }
        TimeSpan TimeOfLastSunMove { get; set; }
        TimeSpan TimeOfLastWatering { get; set; }
        TimeSpan TimeOfNextMisting { get; set; }
        TimeSpan TimeOfNextSunMove { get; set; }
        TimeSpan TimeOfNextWatering { get; set; }
        

        double TimeToNextAction(DateTime lastTime, DateTime nextTime);
    }
}
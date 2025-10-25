namespace ProgramaYA.Models.Dto
{
    public class WeatherViewModel
    {
        public decimal Lat { get; set; } = -9.189967M;
        public decimal Lon { get; set; } = -75.015152M;
        public string Units { get; set; } = "imperial";
        public string Lang { get; set; } = "es";
    public WeatherData? Weather { get; set; }
    public string? RawJson { get; set; }
    public string? Error { get; set; }
    }
}

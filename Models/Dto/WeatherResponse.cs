using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ProgramaYA.Models.Dto
{
    public class WeatherResponse
    {
        [JsonPropertyName("data")]
        public List<WeatherData> Data { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class WeatherData
    {
        [JsonPropertyName("temp")]
        public decimal? Temp { get; set; }
        
        [JsonPropertyName("app_temp")]
        public decimal? AppTemp { get; set; }

        [JsonPropertyName("city_name")]
        public string CityName { get; set; }

        [JsonPropertyName("rh")]
        public decimal? RelativeHumidity { get; set; }
        [JsonPropertyName("country_code")]
        public string? Country_code { get; set; }

        [JsonPropertyName("wind_spd")]
        public decimal? WindSpeed { get; set; }

        [JsonPropertyName("weather")]
        public WeatherDesc Weather { get; set; }
    }

    public class WeatherDesc
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace RDNUGWeather
{

    public class CurrentConditions
    {
        [JsonProperty("weather")]
        public IList<Weather> Weather { get; set; }

        [JsonProperty("main")]
        public Main Main { get; set; }

        [JsonProperty("name")]
        public string CityName { get; set; }
    }

    public class Weather
    {
        [JsonProperty("main")]
        public string Main { get; set; }
    }

    public class Main
    {
        [JsonProperty("temp")]
        public decimal Temperature { get; set; }
    }
}
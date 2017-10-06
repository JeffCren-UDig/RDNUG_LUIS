using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace RDNUGWeather.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string ENDPOINT = "[your bot framework app endpoint]";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;


            string weather;
            WeatherLUIS weatherLUIS = await GetEntityFromLUIS(activity.Text);
            if (weatherLUIS.intents.Count() > 0)
            {
                switch (weatherLUIS.intents[0].intent)
                {
                    case "Weather.GetCondition":
                        weather = await GetCurrentConditions(weatherLUIS.entities[0].entity);
                        break;
                    case "Weather.GetForecast":
                        throw new NotImplementedException();
                    default:
                        weather = "You keep using that word, I do not think it means what you think it means.";
                        break;
                }
            }
            else
            {
                weather = "You keep using that word, I do not think it means what you think it means.";
            }

            await context.PostAsync(weather);

            context.Wait(MessageReceivedAsync);
        }

        private async Task<string> GetCurrentConditions(string city)
        {
            var currentConditions = await OpenWeather.GetWeatherAsync(city);
            if (currentConditions == null)
            {
                return "Not a valid location";
            }

            return string.Format("Current conditions in {1}: {0}. The temperature is {2}\u00B0 F.",
                                    currentConditions.Weather[0].Main,
                                    currentConditions.CityName,
                                    currentConditions.Main.Temperature);
        }

        private static async Task<WeatherLUIS> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            WeatherLUIS Data = new WeatherLUIS();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = ENDPOINT + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<WeatherLUIS>(JsonDataResponse);
                }
            }
            return Data;
        }
    }
}
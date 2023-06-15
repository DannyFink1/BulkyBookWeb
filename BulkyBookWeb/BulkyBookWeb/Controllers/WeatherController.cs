using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System.Runtime.Remoting;

namespace BulkyBookWeb.Controllers
{

    public class WeatherController : Controller
    {
        private readonly HttpClient _httpClient;
        public WeatherController(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(RegionWeatherData obj)
        {

            //ANDERE GEOCODING API VERWENDEN!
            //GEOCODING API
            string coordsAPI = "https://api.opencagedata.com/geocode/v1/json?q=" + obj.Region.RegionName + "&key=6a8a5216bd424dd1a30b351e78199544";
            HttpResponseMessage responseFromCoordsAPI = await _httpClient.GetAsync(coordsAPI);
            if (responseFromCoordsAPI.IsSuccessStatusCode)
            {
                string jsonString = await responseFromCoordsAPI.Content.ReadAsStringAsync();
                JToken token = JToken.Parse(jsonString);

                JObject newObj = new JObject();
                newObj["lat"] = token["results"][0]["geometry"]["lat"];
                newObj["lon"] = token["results"][0]["geometry"]["lng"];

                //API FÜR WETTER

                string weatherAPI = "https://api.openweathermap.org/data/2.5/weather?lat=" + newObj["lat"] + "&lon=" + newObj["lon"] + "&appid=0bd3c9b3aa88d0263f746e687d27b64e";
                HttpResponseMessage responseFromWeatherAPI = await _httpClient.GetAsync(weatherAPI);
                if (responseFromWeatherAPI.IsSuccessStatusCode)
                {
                    jsonString = await responseFromWeatherAPI.Content.ReadAsStringAsync();
                    var weatherData = JsonConvert.DeserializeObject<WeatherData>(jsonString);
                    var data = new RegionWeatherData();
                    data.Weather = weatherData;
                    TempData["ReadyForWeather"] = "yes";
                    TempData["js"] = "yes";
                    return View(data);
                }
                TempData["LocationFound"] = "no";
                return View();
            }
            TempData["LocationFound"] = "no";
            return View();
            
        }
    }
}

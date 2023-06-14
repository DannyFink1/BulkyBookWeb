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
        public async Task<IActionResult> Index(RegionModel obj)
        {

            //ANDERE GEOCODING API VERWENDEN!
            //GEOCODING API
            string coordsAPI = "http://api.openweathermap.org/geo/1.0/direct?q=" + obj.RegionName + "&limit=1&appid=146585a496b1b1b096daae3315024589";
            HttpResponseMessage responseFromCoordsAPI = await _httpClient.GetAsync(coordsAPI);
            string jsonString = await responseFromCoordsAPI.Content.ReadAsStringAsync();
            JToken token = JToken.Parse(jsonString);

            JObject newObj = new JObject();
            newObj["lat"] = token[0]["lat"];
            newObj["lon"] = token[0]["lon"];

            //API FÜR WETTER

            string weatherAPI = "https://api.openweathermap.org/data/3.0/onecall?lat=" + token[0]["lat"] + "&lon=" + token[0]["lon"] + "&appid=0bd3c9b3aa88d0263f746e687d27b64e";
            HttpResponseMessage responseFromWeatherAPI = await _httpClient.GetAsync(weatherAPI);
            jsonString = await responseFromWeatherAPI.Content.ReadAsStringAsync();
            return View();
        }
    }
}

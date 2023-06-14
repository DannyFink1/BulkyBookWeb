using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BulkyBookWeb.Controllers
{
    public class LogInController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly HttpClient _httpClient;
        public LogInController(ApplicationDbContext db, HttpClient httpClient)
        {
            _db = db;
            _httpClient = httpClient;

        }


        public IActionResult Index()
        {
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User obj)
        {
            if (ModelState.IsValid)
            {

                var user = _db.Users.FirstOrDefault(u => u.Username == obj.Username && u.Password == obj.Password && u.Email == obj.Email);
                Console.WriteLine(user);
                if (user !=null)
                {

                    TempData["success"] = "Succesfully Logged In. Hello " + obj.Username + "!";
                    return RedirectToAction("CoinPrice");
                }
                else
                {
                    TempData["error"] = "Credentials not recognized";
                    return View(obj);
                }
                
            }
            return View(obj);
     
        }

        public async Task<IActionResult> CoinPrice()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("https://api.coingecko.com/api/v3/simple/price?ids=bitcoin%2Clitecoin%2Cdogecoin%2Cethereum%2Ctether%2Ctatecoin&vs_currencies=eur");
            if (response.IsSuccessStatusCode)
            {
                // Lesen Sie den Inhalt der Antwort als JSON
                string jsonString = await response.Content.ReadAsStringAsync();
                TempData["lastUpdate"] = response.Headers.Age;

                // Deserialisieren Sie das JSON in ein Objekt
                var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>>>(jsonString);

                List<CoinPriceList> coinPriceList = new List<CoinPriceList>();

                // Iterieren Sie über die Schlüssel-Wert-Paare im data-Objekt
                foreach (var coinEntry in data)
                {
                    string coinName = coinEntry.Key;
                    decimal eurPrice = coinEntry.Value["eur"];

                    // Erstellen Sie ein neues CoinPriceList-Objekt und fügen Sie es zur Liste hinzu
                    CoinPriceList coin = new CoinPriceList
                    {
                        CoinName = coinName,
                        EurPrice = eurPrice
                    };

                    coinPriceList.Add(coin);
                }

                return View(coinPriceList);
            }

            return View();
        }

    }
}

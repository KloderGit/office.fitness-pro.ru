using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using amocrm.library;
using amocrm.library.Interfaces;
using crm.service.database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using office.fitness_pro.ru.Models;

namespace office.fitness_pro.ru.Controllers
{

    public partial class IpInfo
    {
        public string ip { get; set; }
        public bool success { get; set; }
        public string type { get; set; }
        public string continent { get; set; }
        public string continent_code { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
        public string country_flag { get; set; }
        public string country_capital { get; set; }
        public string country_phone { get; set; }
        public string country_neighbours { get; set; }
        public string region { get; set; }
        public string city { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string asn { get; set; }
        public string org { get; set; }
        public string isp { get; set; }
        public string timezone { get; set; }
        public string timezone_name { get; set; }
        public string timezone_dstOffset { get; set; }
        public string timezone_gmtOffset { get; set; }
        public string timezone_gmt { get; set; }
        public string currency { get; set; }
        public string currency_code { get; set; }
        public string currency_symbol { get; set; }
        public string currency_rates { get; set; }
        public string currency_plural { get; set; }
        public int completed_requests { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        CrmDataBaseContext crmDataBase;
        ICrmManager crm;

        public HomeController(ILogger<HomeController> logger, CrmDataBaseContext crmDataBase, ICrmManager crm)
        {
            _logger = logger;
            this.crmDataBase = crmDataBase;
            this.crm = crm;
        }

        public async Task<IActionResult> Index()
        {
            var clientUA = new HttpClient();

            var ress = await clientUA.GetAsync(@"http://free.ipwhois.io/json/195.239.87.70?lang=ru");

            var dfff = await ress.Content.ReadAsAsync<IpInfo>();

            var client = new HttpClient();
            var request = await client.GetAsync(@"https://lc.fitness-pro.ru/Program/ProgramAnnotation").ConfigureAwait(false);

            var lcProgs = await request.Content.ReadAsAsync<IEnumerable<EventsDto>>();


            var task = await crm.CustomFields.ConfigureAwait(false);
            var result = task.Lead[66349].Enums;

            return View( new VM { Amo = result, lC = lcProgs });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


    public class VM
    { 
        public Dictionary<int, string> Amo { get; set; }
        public IEnumerable<EventsDto> lC { get; set; }
    }
}

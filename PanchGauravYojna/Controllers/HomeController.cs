using BL.WebsiteMaster;
using Microsoft.AspNetCore.Mvc;
using MO.WebsiteMaster;
using PanchGauravYojna.Models;
using System.Diagnostics;

namespace PanchGauravYojna.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISliderVM _sliderService;
        public HomeController(ILogger<HomeController> logger, ISliderVM sliderService)
        {
            _logger = logger;
            _sliderService = sliderService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _sliderService.GetHomePageComponent();
            return View(model);
        }

        public IActionResult AboutUs()
        {
            return View();
        }
       
        public IActionResult Event()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult DistrictBooks()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ContactUsState()
        {
            return View();
        }
        
        public IActionResult ContactUsDistrict()
        {
            return View();
        }
    }
}
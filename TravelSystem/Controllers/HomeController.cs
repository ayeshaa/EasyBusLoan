using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelSystem.Models;

namespace TravelSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var result = _context.Vehicles.Include(o => o.VehicleTypes)
                .Include(o => o.VehicleImages).Include(o => o.VehicleRatings).Where(o => o.IsSold == false).Take(8).ToList();
            foreach (var item in result)
            {
                if (item.VehicleImages.Count == 0)
                {
                    var vehicleImage = new VehicleImages
                    {
                        FileNameOnDisk = "logo1.png",
                        VehicleId = item.Id

                    };
                    item.VehicleImages.Add(vehicleImage);
                }
                var stars = 0;
                if (item.VehicleRatings.Count == 0)
                {
                    stars = 5;
                    item.Stars = stars;
                }
                else
                {
                    var count = 0;
                    foreach (var item2 in item.VehicleRatings)
                    {
                        count += item2.Stars;
                    }
                    var totalUsers = item.VehicleRatings.Count;
                    var sumOfUserRating = totalUsers * 5;
                    stars = (count * 5) / sumOfUserRating;
                    item.Stars = stars;
                }

            }
            TempData["Vehicles"] = result.ToList();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
}

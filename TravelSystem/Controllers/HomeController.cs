using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelSystem.Common;
using TravelSystem.Models;
using TravelSystem.Services;

namespace TravelSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly AppSettings _appSettings;
        public HomeController(ApplicationDbContext context, IEmailSender emailSender, IOptions<AppSettings> appSettings)
        {
            _emailSender = emailSender;
            _context = context;
            _appSettings = appSettings.Value;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> Service()
        {
            var service = await _context.Services.FirstOrDefaultAsync();
            return View(service);
        }
        public async Task<IActionResult> PrivacyPolicy()
        {
            var policy = await _context.PrivacyPolicy.FirstOrDefaultAsync();
            return View(policy);
        }
        public async Task<IActionResult> TermsAndConditions()
        {
            var terms = await _context.TermsAndConditions.FirstOrDefaultAsync();
            return View(terms);
        }
        public async Task<IActionResult> Working()
        {
            var work = await _context.HowItWorks.FirstOrDefaultAsync();
            return View(work);
        }
        public async Task<IActionResult> TermsOfUseSite()
        {
            var terms = await _context.TermsOfUseSite.FirstOrDefaultAsync();
            return View(terms);
        }
        public async Task<IActionResult> AboutUs()
        {
            var abt = await _context.About.FirstOrDefaultAsync();
            return View(abt);
        }
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Contact(string Name, string Email, string Phone, string Message)
        {
            var email = _appSettings.FromEmailAddress;
            await _emailSender.SendEmailAsync(email, $"Message From {Email},",
               $"From: {Name}," +
               $"Phone: {Phone}," +
               $"Message: {Message}");
            ViewBag.Message = "Message Sent Successfully";
            return View();
        }
    }
}

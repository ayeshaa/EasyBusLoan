using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelSystem.Common;
using TravelSystem.Models;

namespace TravelSystem.Controllers
{
    public class SellerController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _hostingEnvironment;
        public SellerController(IHostingEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddVehicle()
        {
            TempData["VehicleTypes"] = _context.VehicleTypes.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult AddVehicle(Vehicles vehicle,List<IFormFile> files)
        {
            vehicle.UserId = GetLoggedInUserId();
            _context.Entry(vehicle).State = vehicle.Id > 0 ? Microsoft.EntityFrameworkCore.EntityState.Modified : Microsoft.EntityFrameworkCore.EntityState.Added;
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "VehicleImages");
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var filePath = Path.Combine(uploads, file.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    var objVehicleImage = new VehicleImages
                    {
                        FileNameOnDisk = file.FileName,
                        VehicleId = vehicle.Id
                    };
                    vehicle.VehicleImages.Add(objVehicleImage);
                }
            }
            _context.VehicleImages.AddRange(vehicle.VehicleImages);
            _context.SaveChanges();
            return RedirectToAction("MyVehicles");
        }
        public ActionResult MyVehicles()
        {
            TempData["TotalVehicles"] = _context.Vehicles.Where(o => o.IsSold == false && o.UserId == GetLoggedInUserId()).Count();
            TempData["VehicleTypes"] = _context.VehicleTypes.ToList();
            return View();
        }
        public ActionResult MyVehiclesGrid(string vehicleIds, decimal fromPrice, decimal toPrice)
        {
            string[] selectedVehicles = new string[] { };
            if (!string.IsNullOrEmpty(vehicleIds))
            {
                selectedVehicles = vehicleIds.Split(",");
            }

            var result = _context.Vehicles.Include(o => o.VehicleTypes).Include(o => o.VehicleImages).Include(o => o.VehicleRatings)
                .Where(o => o.IsSold == false && o.UserId == GetLoggedInUserId() && (o.SalesPrice > fromPrice && o.SalesPrice < (toPrice == 0 ? 1000000 : toPrice))).ToList();
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
            List<Vehicles> vehicles = new List<Vehicles>();
            if (selectedVehicles.Count() > 0)
            {
                foreach (var item in selectedVehicles)
                {
                    var vehicle = result.Where(o => o.VehicleTypeId == Convert.ToInt32(item)).ToList();
                    if (vehicle.Count() > 0)
                    {
                        foreach (var s in vehicle)
                        {
                            vehicles.Add(s);
                        }
                    }
                }
                result = vehicles;
            }
            return PartialView(result);
        }
        public IActionResult AllSellers()
        {
            var result = _context.Users.ToList();
            var list = new List<ApplicationUser>();
            foreach(var item in result)
            {
                var find = _context.UserRoles.Any(o => o.RoleId == (int)Role.Seller && o.UserId == item.Id);
                if (find)
                {
                    list.Add(item);
                }
            }
            return View(list);
        }
    }
}
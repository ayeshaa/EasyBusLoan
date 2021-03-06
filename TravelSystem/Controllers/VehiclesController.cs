﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelSystem.Models;

namespace TravelSystem.Controllers
{
    public class VehiclesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var result = _context.Vehicles.Include(o => o.VehicleTypes).Include(o => o.VehicleImages).Include(o => o.VehicleRatings).Where(o => o.IsSold == false).ToList();
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
            return View(result);
        }
        public ActionResult VehicleDetail(int id)
        {
            int userId = GetLoggedInUserId();
            if (userId > 0)
            {
                TempData["IsApplication"] = _context.ApplicantDetails.Any(o => o.UserId == userId);
            }

            var result = _context.Vehicles.Include(o => o.User).Include(o => o.VehicleTypes).Include(o => o.VehicleImages)
                .Include(o => o.VehicleRatings).ThenInclude(o => o.User).FirstOrDefault(o => o.Id == id);
            if (result != null)
            {
                if (result.VehicleImages.Count() == 0)
                {
                    var vehicleImage = new VehicleImages
                    {
                        FileNameOnDisk = "logo1.png",
                        VehicleId = result.Id

                    };
                    result.VehicleImages.Add(vehicleImage);
                }
                var stars = 0;
                if (result.VehicleRatings.Count == 0)
                {
                    stars = 5;
                    result.Stars = stars;
                }
                else
                {
                    var count = 0;
                    foreach (var result2 in result.VehicleRatings)
                    {
                        count += result2.Stars;
                    }
                    var totalUsers = result.VehicleRatings.Count;
                    var sumOfUserRating = totalUsers * 5;
                    stars = (count * 5) / sumOfUserRating;
                    result.Stars = stars;
                }
            }
            return View(result);
        }
        public ActionResult VehicleReview(VehicleRatings vehicleRatings)
        {
            int userId = GetLoggedInUserId();
            if (userId > 0)
            {
                var vehicleRating = _context.VehicleRatings.FirstOrDefault(o => o.VehicleId == vehicleRatings.VehicleId && o.UserId == userId);
                if (vehicleRating == null)
                {
                    vehicleRatings.UserId = userId;
                    _context.VehicleRatings.Add(vehicleRatings);
                    _context.SaveChanges();
                }
                return RedirectToAction("VehicleDetail", "Vehicles", new { id = vehicleRatings.VehicleId });
            }
            ViewBag.VehicleId = vehicleRatings.VehicleId;
            return RedirectToAction("LogIn", "User", new { vehicleId = vehicleRatings.VehicleId });
        }
        public ActionResult Inventory(int? page,string searchText, string searchModel, DateTime? searchYear)
        {
            int skip = 0;
            int take = 12;
            if (!page.HasValue)
            {
                page = 1;
            }
            else
            {
                if (page == 1)
                {
                    skip = 0;
                }
                else
                {
                    skip = (page.Value * take) - take;
                }
            }
            ViewBag.TotalVehicles = _context.Vehicles.Count();
            ViewBag.SearchText = searchText;
            ViewBag.SearchModel = searchModel;
            ViewBag.SearchYear = searchYear;
            ViewBag.TakeVehicles = take;
            ViewBag.SkipVehicles = skip;
            TempData["TotalVehicles"] = _context.Vehicles.Where(o => o.IsSold == false).Count();
            TempData["VehicleTypes"] = _context.VehicleTypes.ToList();
            return View();
        }
        public ActionResult InventoryGrid(string vehicleIds, decimal fromPrice, decimal toPrice, string searchText, string searchModel, DateTime? searchYear,int take,int skip)
        {
            string[] selectedVehicles = new string[] { };
            
            ViewBag.TotalVehicles = _context.Vehicles.Count();
            if (!string.IsNullOrEmpty(vehicleIds))
            {
                selectedVehicles = vehicleIds.Split(",");
            }
            ViewBag.SearchText = searchText;
            var result = new List<Vehicles>();
            searchText = searchText == null ? string.Empty : searchText;
             result = _context.Vehicles.Include(o => o.VehicleTypes)
               .Include(o => o.VehicleImages).Include(o => o.VehicleRatings)
               .Where(o => (o.VehicleTypes.VehicleTypeName.Contains(searchText) || o.Vinnumber.Contains(searchText) 
               || o.Location.Contains(searchText) || o.Mileage.Contains(searchText) || o.CityOrState.Contains(searchText) || o.SalesPrice.Equals(searchText)) 
               && (o.Model.Contains(string.IsNullOrEmpty(searchModel) ? string.Empty : searchModel))
               && (o.IsSold == false) && (o.SalesPrice > fromPrice && o.SalesPrice < (toPrice == 0 ? 1000000 : toPrice))).Skip(skip).Take(take).ToList();
            if (searchYear.HasValue)
            {
                result = result.Where(o => o.MakeYear.Year == searchYear.Value.Year).ToList();
            }
            TempData["TotalVehicles"] = result.Count();
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
    }
}
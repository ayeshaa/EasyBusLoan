using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelSystem.Common;
using TravelSystem.Models;

namespace TravelSystem.Controllers
{
    public class LendorController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public LendorController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AllLendors()
        {
            var result = _context.Users.ToList();
            var list = new List<ApplicationUser>();
            foreach (var item in result)
            {
                var find = _context.UserRoles.Any(o => o.RoleId == (int)Role.Lendor && o.UserId == item.Id);
                if (find)
                {
                    list.Add(item);
                }
            }
            return View(list);
        }
        public async Task<ActionResult> PendingApplications()
        {
            var result = new List<ApplicantDetails>();
            if (User.Identity.IsAuthenticated)
            {
                result = await _context.ApplicantDetails.Include(o => o.Vehicle).ThenInclude(o => o.VehicleImages).Include(o=>o.Vehicle)
                    .ThenInclude(o => o.VehicleTypes).Include(o=>o.ApplicantVehicles).ThenInclude(o=>o.VehicleTypes)
                    .Include(o => o.User).Include(o => o.ApplicantsFinancedEquipments)
               .Include(o => o.ApplicantsCreditReferences).Include(o => o.ApplicantsGuarantors)
               .Include(o => o.ApplicantsInsurances).Where(o=>o.ApprovedByUserId == null && o.ApprovedDate == null).ToListAsync();
            }
            return View(result);
        }
        public async Task<ActionResult> ApplicantDetail(int id)
        {
            var result = new ApplicantDetails();
            if (User.Identity.IsAuthenticated)
            {
                result = await _context.ApplicantDetails.Include(o => o.Vehicle).ThenInclude(o => o.VehicleImages).Include(o => o.Vehicle)
                    .ThenInclude(o => o.VehicleTypes).Include(o => o.Vehicle).ThenInclude(o=>o.VehicleRatings)
                    .Include(o => o.User).Include(o => o.ApplicantsFinancedEquipments).Include(o => o.ApplicantVehicles).ThenInclude(o => o.VehicleTypes)
               .Include(o => o.ApplicantsCreditReferences).Include(o => o.ApplicantsGuarantors)
               .Include(o => o.ApplicantsInsurances).Where(o=>o.Id == id).FirstOrDefaultAsync();
            }
            return View(result);
        }
        public ActionResult ApproveApplication(int id)
        {
            var result = _context.ApplicantDetails.FirstOrDefault(o => o.Id == id);
            if (User.Identity.IsAuthenticated)
            {
                result.ApprovedByUserId = GetLoggedInUserId();
                result.ApprovedDate = DateTime.Now;
            }
            _context.Entry(result).State = EntityState.Modified;
            _context.SaveChanges();
            TempData["Success"] = "Application is approved successfully";
            return RedirectToAction("PendingApplications");
        }
    }
}
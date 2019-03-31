using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using TravelSystem.Common;
using TravelSystem.Models;
using TravelSystem.Models.AccountViewModels;

namespace TravelSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        public AdminController(IHostingEnvironment hostingEnvironment, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        public IActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(Admins admin)
        {
            var adminModel = await _context.Admins.FirstOrDefaultAsync(o => o.UserName == admin.UserName && o.Password == admin.Password);
            if (adminModel != null)
            {
                HttpContext.Session.SetInt32("Id", adminModel.Id);
                HttpContext.Session.SetString("UserName", adminModel.UserName);
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                TempData["Error"] = "Invalid Username or Password";
                return RedirectToAction("Index", "Admin");
            }
        }
        public ActionResult Dashboard()
        {
            var adminId = HttpContext.Session.GetInt32("Id");
            if (adminId.HasValue)
            {
                return View();
            }
            else
            {
                return RedirectToAction("LogIn", "Admin");
            }
        }
        public ActionResult ChangePassword(string email)
        {
            var changePasswordModel = new ChangePasswordViewModel
            {
                Email = email
        };
            return View(changePasswordModel);
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (ModelState.IsValid)
        {
            if (user != null)
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);
            }
            await _userManager.UpdateAsync(user);
        }

        // If we got this far, something failed, redisplay form
        return RedirectToAction("Users","Admin");
    }
    public ActionResult AddBlog(int? id)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var blog = new Blogs();
            if (id.HasValue)
            {
                blog = _context.Blogs.FirstOrDefault(o => o.Id == id);
            }
            return View(blog);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult SaveBlog(Blogs blogs)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            blogs.AdminId = adminId.Value;
            blogs.CreatedDate = blogs.Id > 0 ? blogs.CreatedDate : DateTime.Now;
            _context.Entry(blogs).State = blogs.Id > 0 ? EntityState.Modified : EntityState.Added;
            _context.SaveChanges();
            return RedirectToAction("Blogs");
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult Blogs()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            return View();
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public async Task<IActionResult> GetAllBlogs()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var result = await _context.Blogs.ToListAsync();
            return Json(result);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult BlogCategories()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            return View();
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult AddBlogCategories(int? id)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var blogCategory = new BlogCategories();
            if (id.HasValue)
            {
                blogCategory = _context.BlogCategories.FirstOrDefault(o => o.Id == id);
            }
            return View(blogCategory);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public async Task<IActionResult> GetAllBlogCategories()
    {
        var result = await _context.BlogCategories.ToListAsync();
        return Json(result);
    }
    public ActionResult SaveBlogCategories(BlogCategories blogCategories)
    {
        _context.Entry(blogCategories).State = blogCategories.Id > 0 ? EntityState.Modified : EntityState.Added;
        _context.SaveChanges();
        return RedirectToAction("BlogCategories");
    }
    public async Task<ActionResult> DeleteBlogCategory(int id)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var blogCategory = _context.BlogCategories.FirstOrDefault(o => o.Id == id);
            if (blogCategory != null)
            {
                _context.BlogCategories.Remove(blogCategory);
                await _context.SaveChangesAsync();
            }
            return Json(true);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public async Task<ActionResult> DeleteBlog(int id)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var blog = _context.Blogs.FirstOrDefault(o => o.Id == id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
            }
            return Json(true);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");

    }
    public ActionResult LoanApplications()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            return View();
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult Users()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            return View();
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public IActionResult GetAllUsers()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var result = _context.Users.ToList();
            var list = new List<ApplicationUser>();

            foreach (var item in result)
            {
                var find = _context.UserRoles.FirstOrDefault(o => o.UserId == item.Id);
                switch (find.RoleId)
                {
                    case (int)Role.Lendor:
                        item.IsLendor = true;
                        break;
                    case (int)Role.Buyer:
                        item.IsBuyer = true;
                        break;
                    case (int)Role.Seller:
                        item.IsSeller = true;
                        break;
                }
                list.Add(item);
            }
            return Json(list);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
        public ActionResult Detail(int id)
        {
            var user = _context.Users.FirstOrDefault(o => o.Id == id);
            return View(user);
        }
    public async Task<IActionResult> BlockUser(int id)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            await _context.Database.ExecuteSqlCommandAsync("spBlockUser @Id = {0}", id);
            return Json(true);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult Payments()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            return View();
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public async Task<IActionResult> GetAllPayments()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var result = await _context.spGetVehiclePaymentsResult.FromSql("spGetVehiclePayments").ToListAsync();
            return Json(result);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public async Task<IActionResult> GetAllVehicles()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var result = await _context.spGetAllVehiclesResult.FromSql("spGetAllVehicles").ToListAsync();
            return Json(result);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult Vehicles()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            return View();
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public async Task<ActionResult> DeleteVehicle(int id)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var vehicle = _context.Vehicles.FirstOrDefault(o => o.Id == id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
            return Json(true);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult AddVehicle(int? id)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var vehicle = new Vehicles();
            if (id.HasValue)
            {
                vehicle = _context.Vehicles.Include(o => o.VehicleImages).FirstOrDefault(o => o.Id == id);
            }
            return View(vehicle);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }

    [HttpPost]
    public IActionResult SaveVehicle(Vehicles vehicle, List<IFormFile> files)
    {
        vehicle.UserId = 1014;
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
        if (files.Count() > 0)
        {
            _context.Database.ExecuteSqlCommand("Delete From VehicleImages Where VehicleId = {0}", vehicle.Id);
            _context.VehicleImages.AddRange(vehicle.VehicleImages);
        }

        _context.SaveChanges();
        return RedirectToAction("Vehicles");
    }
    public IActionResult GetVehicleTypes()
    {
        return Json(_context.VehicleTypes.ToList());
    }
    public ActionResult VehicleTypes()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            return View();
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult AddVehicleType(int? id)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var vehicleType = new VehicleTypes();
            if (id.HasValue)
            {
                vehicleType = _context.VehicleTypes.FirstOrDefault(o => o.Id == id);
            }
            return View(vehicleType);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public IActionResult SaveVehicleType(VehicleTypes vehicleTypes)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            _context.Entry(vehicleTypes).State = vehicleTypes.Id > 0 ? EntityState.Modified : EntityState.Added;
            _context.SaveChanges();
            return RedirectToAction("VehicleTypes");
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public async Task<IActionResult> DeleteVehicleType(int id)
    {
        var admin = HttpContext.Session.GetInt32("Id");
        if (admin.HasValue)
        {
            var vehicleTypes = _context.VehicleTypes.FirstOrDefault(o => o.Id == id);
            if (vehicleTypes != null)
            {
                _context.VehicleTypes.Remove(vehicleTypes);
                await _context.SaveChangesAsync();
            }
            return Json(true);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult ApplicantDetails()
    {
        var admin = HttpContext.Session.GetInt32("Id");
        if (admin.HasValue)
        {
            return View();
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public IActionResult GetAllApplicantDetails()
    {
        return Json(_context.ApplicantDetails.ToList());
    }
    public ActionResult AddApplicant(int? id)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var applicant = new ApplicantDetails();
            if (id.HasValue)
            {
                applicant = _context.ApplicantDetails.FirstOrDefault(o => o.Id == id);
            }
            return View(applicant);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public IActionResult SaveApplicantDetails(ApplicantDetails applicant)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            _context.Entry(applicant).State = applicant.Id > 0 ? EntityState.Modified : EntityState.Added;
            _context.SaveChanges();
            return RedirectToAction("ApplicantDetails");
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult Lendors()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            return View();
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public IActionResult GetAllLendors()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var result = _context.Users.ToList();
            var list = new List<ApplicationUser>();

            foreach (var item in result)
            {
                var find = _context.UserRoles.FirstOrDefault(o => o.UserId == item.Id);
                if (find.RoleId == (int)Role.Lendor)
                {
                    switch (find.RoleId)
                    {
                        case (int)Role.Lendor:
                            item.IsLendor = true;
                            break;
                        case (int)Role.Buyer:
                            item.IsBuyer = true;
                            break;
                        case (int)Role.Seller:
                            item.IsSeller = true;
                            break;
                    }
                    list.Add(item);
                }

            }
            return Json(list);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult AddWorking()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var work = _context.HowItWorks.FirstOrDefault();
            return View(work);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult SaveWorking(HowItWorks works)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            _context.Entry(works).State = works.Id > 0 ? EntityState.Modified : EntityState.Added;
            _context.SaveChanges();
            return RedirectToAction("AddWorking");
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult AddPrivacyPolicy()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var policy = _context.PrivacyPolicy.FirstOrDefault();
            return View(policy);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult SavePrivacyPolicy(PrivacyPolicy policy)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            _context.Entry(policy).State = policy.Id > 0 ? EntityState.Modified : EntityState.Added;
            _context.SaveChanges();
            return RedirectToAction("AddPrivacyPolicy");
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult AddTermsAndConditions()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var terms = _context.TermsAndConditions.FirstOrDefault();
            return View(terms);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult SaveTermsAndConditions(TermsAndConditions terms)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            _context.Entry(terms).State = terms.Id > 0 ? EntityState.Modified : EntityState.Added;
            _context.SaveChanges();
            return RedirectToAction("AddTermsAndConditions");
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult AddTermsOfUseSite()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var terms = _context.TermsOfUseSite.FirstOrDefault();
            return View(terms);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult SaveTermsOfUseSite(TermsOfUseSite terms)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            _context.Entry(terms).State = terms.Id > 0 ? EntityState.Modified : EntityState.Added;
            _context.SaveChanges();
            return RedirectToAction("AddTermsOfUseSite");
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult AddServices()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var service = _context.Services.FirstOrDefault();
            return View(service);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult SaveServices(Service service)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            _context.Entry(service).State = service.Id > 0 ? EntityState.Modified : EntityState.Added;
            _context.SaveChanges();
            return RedirectToAction("AddServices");
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult AddAbout()
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            var abt = _context.About.FirstOrDefault();
            return View(abt);
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
    public ActionResult SaveAbout(AboutUs abt)
    {
        var adminId = HttpContext.Session.GetInt32("Id");
        if (adminId.HasValue)
        {
            _context.Entry(abt).State = abt.Id > 0 ? EntityState.Modified : EntityState.Added;
            _context.SaveChanges();
            return RedirectToAction("AddAbout");
        }
        else
        {
            return RedirectToAction("LogIn", "Admin");
        }
    }
}
}
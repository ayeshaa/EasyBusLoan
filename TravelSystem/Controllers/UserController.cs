using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelSystem.Models;
using TravelSystem.Models.AccountViewModels;
using TravelSystem.Services;

namespace TravelSystem.Controllers
{
    public class UserController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _hostingEnvironment;
        string machineName = GetIPAddress(Dns.GetHostName()).ToString();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public UserController(IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<UserController> logger, ApplicationDbContext context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            // ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                    if (user.IsBlock)
                    {
                        ModelState.AddModelError(string.Empty, "You are blocked, Please contact support");
                        return View(model);
                    }
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public async Task<IActionResult> Register(string finance)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            HttpContext.Session.SetObjectAsJson("Cart", new List<VehiclesCarts>());
            TempData["Finance"] = finance;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, List<IFormFile> files, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var vehicles = new List<Vehicles>();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    CompanyName = model.CompanyName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    string ctoken = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                    string ctokenLink = Url.Action("ConfirmEmail", "User", new
                    {
                        userId = user.Id,
                        token = ctoken
                    }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{ctokenLink}'>link</a>");
                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    TempData["Success"] = "An email has been sent Please verify your email address";
                    var selectedRole = "";
                    if (model.IsSeller)
                    {
                        selectedRole = "Seller";
                    }
                    if (model.IsLendor)
                    {
                        selectedRole = "Lendor";
                    }
                    if (model.IsBuyer)
                    {
                        selectedRole = "Buyer";
                    }
                    var role = await _userManager.AddToRoleAsync(user, selectedRole);
                    if (!role.Succeeded)
                    {
                        ModelState.AddModelError("", role.Errors.ToString());
                        return View();
                    }
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    user.Vehicles = model.Vehicles;
                    if (model.IsSeller)
                    {
                        SaveSeller(user, files);
                    }
                    if (model.IsBuyer)
                    {
                        user.ApplicantDetails = model.ApplicantDetails;
                        SaveBuyer(user);
                    }
                    return View(model);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public bool AddToCart(int id)
        {
            var findVehicleCart = _context.VehiclesCarts.FirstOrDefault(o => o.MachineName == machineName && o.VehicleId == id);
            if (findVehicleCart == null)
            {
                var vehicleCart = new VehiclesCarts
                {
                    VehicleId = id,
                    MachineName = machineName,
                };
                if (HttpContext.Session.GetInt32("Id") > 0)
                {
                    vehicleCart.UserId = HttpContext.Session.GetInt32("Id").Value;
                }
                _context.VehiclesCarts.Add(vehicleCart);
                _context.SaveChanges();
            }
            return true;
        }
        public IActionResult BuyerRegistration()
        {
            var vehicles = _context.Vehicles.Include(o => o.VehicleTypes)
                .Include(o => o.VehicleImages).Include(o => o.VehicleRatings).Where(o => o.IsSold == false).ToList();
            foreach (var item in vehicles)
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
            TempData["VehicleTypes"] = _context.VehicleTypes.ToList();
            TempData["Vehicles"] = vehicles;
            return View();
        }
        public void SaveBuyer(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                if (user.Vehicles.Count > 0)
                {
                    foreach (var item in user.Vehicles)
                    {
                        if (item.IsChecked)
                        {
                            var applicationDetail = user.ApplicantDetails.FirstOrDefault();
                            applicationDetail.VehicleId = item.Id;
                            user.ApplicantDetails.Add(applicationDetail);
                        }
                    }
                }
                if (user.Vehicles.Any(o => o.IsChecked))
                {
                    for (var i = 0; i < user.ApplicantDetails.Count; i++)
                    {
                        if (i == 0)
                        {
                            user.ApplicantDetails.Remove(user.ApplicantDetails[i]);
                        }
                    }
                }

                if (user.Vehicles.Count > 0)
                {
                    foreach (var item in user.Vehicles.ToList())
                    {
                        user.Vehicles.Remove(item);
                    }
                }
                foreach (var item in user.ApplicantDetails)
                {
                    item.UserId = user.Id;
                }
                _context.ApplicantDetails.AddRange(user.ApplicantDetails);
                foreach (var item in user.ApplicantDetails)
                {
                    foreach (var item2 in item.ApplicantVehicles)
                    {
                        item2.ApplicantId = item.Id;
                    }
                    foreach (var item2 in item.ApplicantsCreditReferences)
                    {
                        if (item2.BankName == string.Empty && item2.AccountNumber == string.Empty)
                        {
                            item.ApplicantsCreditReferences.Remove(item2);
                        }
                        else
                        {
                            item2.ApplicantId = item.Id;
                        }
                    }
                    foreach (var item2 in item.ApplicantsFinancedEquipments)
                    {
                        item2.ApplicantId = item.Id;
                    }
                    foreach (var item2 in item.ApplicantsGuarantors)
                    {
                        item2.ApplicantId = item.Id;
                    }
                    foreach (var item2 in item.ApplicantsInsurances)
                    {
                        item2.ApplicantId = item.Id;
                    }
                    if (item.ApplicantVehicles.Count > 0)
                    {
                        _context.ApplicantVehicles.AddRange(item.ApplicantVehicles);
                    }
                    if (item.ApplicantsCreditReferences.Count > 0)
                    {
                        _context.ApplicantsCreditReferences.AddRange(item.ApplicantsCreditReferences);
                    }
                    if (item.ApplicantsFinancedEquipments.Count > 0)
                    {
                        _context.ApplicantsFinancedEquipments.AddRange(item.ApplicantsFinancedEquipments);
                    }
                    if (item.ApplicantsGuarantors.Count > 0)
                    {
                        _context.ApplicantsGuarantors.AddRange(item.ApplicantsGuarantors);
                    }
                    if (item.ApplicantsInsurances.Count > 0)
                    {
                        _context.ApplicantsInsurances.AddRange(item.ApplicantsInsurances);
                    }
                }

                _context.SaveChanges();
                //SetSession(user);
                // return RedirectToAction("Index", "Buyer");
            }
            //return RedirectToAction("Register", "User");
        }
        public IActionResult SellerRegistration()
        {
            TempData["VehicleTypes"] = _context.VehicleTypes.ToList();
            return View();
        }
        public void SaveSeller(ApplicationUser user, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in user.Vehicles)
                {
                    item.UserId = user.Id;
                    _context.Entry(item).State = item.Id > 0 ? Microsoft.EntityFrameworkCore.EntityState.Modified : Microsoft.EntityFrameworkCore.EntityState.Added;
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
                                VehicleId = item.Id
                            };
                            item.VehicleImages.Add(objVehicleImage);
                        }
                    }
                    _context.VehicleImages.AddRange(item.VehicleImages);
                }
                _context.SaveChanges();
            }
        }
        public IActionResult LendorRegistration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SaveLendor(ApplicationUser user)
        {
            //user.IsLendor = true;
            //_context.Entry(user).State = user.Id > 0 ? Microsoft.EntityFrameworkCore.EntityState.Modified : Microsoft.EntityFrameworkCore.EntityState.Added;
            //_context.SaveChanges();
            //SetSession(user);
            return RedirectToAction("Index", "Lendor");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        public IActionResult UserDetail(int id)
        {
            return View(_context.Users.Where(o => o.Id == id).FirstOrDefault());
        }




        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        public async Task<IActionResult> ProfileSettings()
        {
            var model = new RegisterViewModel();

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user != null)
            {
                model = new RegisterViewModel
                {
                    CompanyName = user.CompanyName,
                    FullName = user.FullName,
                    Phone = user.Phone,
                };
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ProfileSettings(RegisterViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    user.CompanyName = model.CompanyName;
                    user.Phone = model.Phone;
                    user.FullName = model.FullName;
                }
                await _userManager.UpdateAsync(user);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user,model.NewPassword);
                }
                await _userManager.UpdateAsync(user);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        #endregion
    }
}
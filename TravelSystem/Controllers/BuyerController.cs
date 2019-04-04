using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using TravelSystem.Models;
using TravelSystem.Models.AccountViewModels;

namespace TravelSystem.Controllers
{
    public class BuyerController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public BuyerController(ApplicationDbContext context)
        {
            _context = context;
        }
        string machineName = GetIPAddress(Dns.GetHostName()).ToString();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ConnectToApplication(int vehicleId)
        {
            int userId = GetLoggedInUserId();

            if (userId > 0)
            {
                var applicant = _context.ApplicantDetails.FirstOrDefault(o => o.UserId == userId);
                if (applicant != null)
                {
                    var applicantWithVehicle = _context.ApplicantDetails.FirstOrDefault(o => o.UserId == userId && o.VehicleId == vehicleId);
                    if (applicantWithVehicle == null)
                    {
                        _context.Database.ExecuteSqlCommand("spCopyApplicantDetail @VehicleId = {0},@ApplicantId = {1}", vehicleId, applicant.Id);
                    }
                }
            }
            _context.SaveChanges();
            return RedirectToAction("VehicleDetail", "Vehicles", new { id = vehicleId });
        }
        public async Task<ActionResult> MyApplications()
        {
            int userId = GetLoggedInUserId();
            var result = new List<ApplicantDetails>();
            if (userId > 0)
            {
                result = await _context.ApplicantDetails.Include(o => o.Vehicle).ThenInclude(o => o.VehicleImages).Include(o => o.Vehicle)
                    .ThenInclude(o => o.VehicleTypes)
                    .Include(o => o.User).Include(o => o.ApplicantsFinancedEquipments)
               .Include(o => o.ApplicantsCreditReferences).Include(o => o.ApplicantsGuarantors)
               .Include(o => o.ApplicantsInsurances).Where(o => o.UserId == userId).ToListAsync();
            }
            return View(result);
        }
        public async Task<ActionResult> ApplicantDetail(int id)
        {
            int userId = GetLoggedInUserId();
            var result = new ApplicantDetails();
            if (userId > 0)
            {
                result = await _context.ApplicantDetails.Include(o => o.Vehicle).ThenInclude(o => o.VehicleImages).Include(o => o.Vehicle)
                    .ThenInclude(o => o.VehicleTypes).Include(o => o.Vehicle).ThenInclude(o => o.VehicleRatings)
                    .Include(o => o.User).Include(o => o.ApplicantsFinancedEquipments)
               .Include(o => o.ApplicantsCreditReferences).Include(o => o.ApplicantsGuarantors)
               .Include(o => o.ApplicantsInsurances).Where(o => o.Id == id).FirstOrDefaultAsync();
            }
            return View(result);
        }
        public JsonResult AddToCart(int id)
        {
            int userId = GetLoggedInUserId();
            var cart = HttpContext.Session.GetObjectFromJson<List<VehiclesCarts>>("Cart").ToList();
            var findVehicle = cart.Any(o => o.VehicleId == id && (o.MachineName.Contains(machineName) || o.UserId == userId));
            if (!findVehicle)
            {
                var vehicleCart = new VehiclesCarts()
                {
                    Id = cart.Count() + 1,
                    MachineName = machineName,
                    UserId = userId > 0 ? userId : (int?)null,
                    VehicleId = id,
                    SpGetVehicleDetail = _context.spGetVehicleDetail.FromSql("spGetVehicleDetail @VehicleId = {0}", id).FirstOrDefault()
                };
                cart.Add(vehicleCart);
                HttpContext.Session.SetObjectAsJson("Cart", cart.ToList());
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        public ActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<VehiclesCarts>>("Cart").ToList();
            var findCart = cart.FirstOrDefault(o => o.Id == id);
            if (findCart != null)
            {
                cart.Remove(findCart);
                HttpContext.Session.SetObjectAsJson("Cart", cart.ToList());
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        public IActionResult Charge(int PaymentId,string stripeEmail, string stripeToken,int UserId,OrderAddresses orderAddress)
        {
            int userId = GetLoggedInUserId();
            TempData["Id"] = userId;
                var vehiclesInCart = HttpContext.Session.GetObjectFromJson<List<VehiclesCarts>>("Cart").ToList();
                var cart = vehiclesInCart.Where(o => o.MachineName.Contains(machineName) || o.UserId == userId).ToList();
                var amount = cart.Select(o => o.SpGetVehicleDetail.SalesPrice).Sum();
                var charges = (cart.Select(o => o.SpGetVehicleDetail.SalesPrice).Sum() * 4) / 100;
                var totalAmount = (amount + charges) * 100;
                var user = _context.Users.FirstOrDefault(o => o.Id == userId);
                var customerService = new StripeCustomerService();
                var chargeService = new StripeChargeService();
                var customer = customerService.Create(new StripeCustomerCreateOptions
                {
                    Email = stripeEmail,
                    SourceToken = stripeToken
                });
                var charge = chargeService.Create(new StripeChargeCreateOptions
                {
                    Amount = Convert.ToInt32(totalAmount),
                    Currency = "usd",
                    CustomerId = customer.Id,
                    Description = user != null ? (user.FullName + "_" + user.Id) : machineName + " has been made transaction"
                });
            var payment = new Payments
                {
                    StripeCustomerId = customer.Id,
                    TotalAmount = totalAmount,
                    CreatedDate = DateTime.Now,
                    UserId = user != null ? user.Id : (int?)null,
                    IpAddress = user != null ? null : machineName
                };
                _context.Payments.Add(payment);
                _context.SaveChanges();
                var order = new OrderAddresses
                {
                    PaymentId = payment.Id,
                    BillingName = orderAddress.BillingName,
                    BillingCompany = orderAddress.BillingCompany,
                    BillingAddress1 = orderAddress.BillingAddress1,
                    BillingAddress2 = orderAddress.BillingAddress2,
                    BillingCity = orderAddress.BillingCity,
                    BillingCountry = orderAddress.BillingCountry,
                    BillingZipCode = orderAddress.BillingZipCode,
                    ShippingName = orderAddress.ShippingName,
                    ShippingCompany = orderAddress.ShippingCompany,
                    ShippingAddress1 = orderAddress.ShippingAddress1,
                    ShippingAddress2 = orderAddress.ShippingAddress2,
                    ShippingCity = orderAddress.ShippingCity,
                    ShippingCountry = orderAddress.ShippingCountry,
                    ShippingZipCode = orderAddress.ShippingZipCode

                };
                _context.OrderAddresses.Add(order);
                _context.SaveChanges();
                var paymentVehicleList = new List<PaymentVehicles>();
                foreach (var item in cart)
                {
                    var paymentVehicle = new PaymentVehicles
                    {
                        PaymentId = payment.Id,
                        VehicleId = item.VehicleId,
                    };
                    paymentVehicleList.Add(paymentVehicle);
                }
                _context.PaymentVehicles.AddRange(paymentVehicleList);
                HttpContext.Session.SetObjectAsJson("Cart", new List<VehiclesCarts>());
                _context.SaveChanges();
                TempData["Success"] = "Transaction has been made successfully";
                return RedirectToAction("Cart", "Buyer");
        }
        public ActionResult OrderAddresses(int PaymentId)
        {
            var result = _context.OrderAddresses.FirstOrDefault(o=>o.PaymentId==PaymentId);
            return View(result);
        }
        public ActionResult Cart()
        {
            int userId = GetLoggedInUserId();
            TempData["Id"] = userId;
            var result = HttpContext.Session.GetObjectFromJson<List<VehiclesCarts>>("Cart").ToList();
            return View(result);
        }
    }
}
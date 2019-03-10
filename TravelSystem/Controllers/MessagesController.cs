using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TravelSystem.Models;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Claims;
using TravelSystem.ViewModel;
using System.IO;
using Microsoft.AspNetCore.Http;
using Wangkanai.Detection;

namespace TravelSystem.Controllers
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class MessagesController : BaseController
    {
        private ApplicationDbContext _context;
        private readonly IDetection _detection;
        private IHostingEnvironment _hostingEnvironment;
        string machineName = GetIPAddress(Dns.GetHostName()).ToString();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public MessagesController(IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<MessagesController> logger, ApplicationDbContext context, IDetection detection)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _detection = detection;
        }
        //GET: Chat
        public ActionResult Index(int id)
        {

            var userId = GetLoggedInUserId();
            if (userId.ToString() == null)
            {
                return RedirectToAction("Index", "Home", null);
            }
            if (id != null)
            {
                if (userId == id)
                {
                    id = 0;
                }
            }
            var UserId = GetLoggedInUserId();
            var context = new ApplicationDbContext();

            using (_context = new ApplicationDbContext())
            {
                var UserPIC = _context.Users.Where(x => x.Id == UserId).FirstOrDefault();

                if (UserPIC.Phone != null && UserPIC.Photo.Length > 1)
                {

                    TempData["ActiveUserpic"] = UserPIC.Photo;
                }
                else
                {
                    TempData["ActiveUserpic"] = "Profileplaceholder.png";
                }
            }

            if (id != 0)
            {
                using (_context = new ApplicationDbContext())
                {
                    var U = _context.Users.Where(x => x.Id == id).FirstOrDefault();
                    Conversations con = _context.Conversation.Where(x => x.From == id || x.To == id).FirstOrDefault();
                    if (con == null)
                    {
                        Conversations dcon = new Conversations();
                        dcon.To = id;
                        dcon.From = userId;
                        dcon.IsSeen = true;
                        dcon.Date = DateTime.Now;
                        dcon.Text = "Now You Can Start Chat With Me";

                        _context.Conversation.Add(dcon);
                        _context.SaveChanges();
                    }

                    ViewBag.ActiveUserId = id;
                    ViewBag.ActiveUserName = U.FullName;
                    if (U.Photo != null)
                    {
                        if (U.Photo.Length < 1)
                        {
                            ViewBag.ActiveUserpic = U.Photo;
                        }
                    }
                    if (U.Photo == null)
                    {
                        ViewBag.ActiveUserpic = "Profileplaceholder.png";
                    }

                    //var UserId = User.Identity.GetUserId();
                    //var UserPIC = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();
                    //if (UserPIC.ProfilePic == null || UserPIC.ProfilePic.Length<1)
                    //{
                    //    Session["UserDP"] = "Profileplaceholder.png";
                    //}
                    //else
                    //{
                    //    Session["UserDP"] = UserPIC.ProfilePic;
                    //}
                }
            }

            ViewBag.UserId = userId;
            return View();

        }

        public ActionResult _AddedUsers()
        {
            List<Conversations> addedUsr = new List<Conversations>();
            List<Users> refinedUsr = new List<Users>();
            List<UserConversationVM> query = new List<UserConversationVM>();
            var userId = GetLoggedInUserId();
            using (_context = new ApplicationDbContext())
            {

                var results = _context.Conversation.Where(x => x.From == userId || x.To == userId).ToList();
                foreach (var item in results)
                {
                    bool alreadyExists = addedUsr.Any(x => x.From == item.From && x.To == item.To);
                    if (addedUsr.Count() == 0)
                    {
                        addedUsr.Add(item);
                    }
                    else if (alreadyExists == false)
                    {
                        bool alreadyExists2 = addedUsr.Any(x => x.To == item.From && x.From == item.To);
                        if (alreadyExists2 == false)
                        {
                            addedUsr.Add(item);
                        }
                    }

                }
                addedUsr.ToList();


                foreach (var item in addedUsr)
                {
                    var usr = _context.Users.Where(x => x.Id != userId && (x.Id == item.From || x.Id == item.To)).FirstOrDefault();
                    var Concount = _context.Conversation.Where(a => a.To == userId && a.From == usr.Id && a.IsSeen == false);
                    int count = 0;
                    if (Concount != null)
                    {
                        count = Concount.Count();
                    }
                    UserConversationVM con = new UserConversationVM();
                    con.Id = usr.Id;
                    con.ImageUrl = usr.Photo;
                    if (usr.IsLogin == null)
                    {
                        con.IsLogin = true;
                    }
                    else
                    {
                        con.IsLogin = (bool)usr.IsLogin;
                    }
                    con.UserName = usr.FullName;
                    con.count = count;
                    query.Add(con);


                }


                //var query = from c in db.AspNetUsers
                //            where c.Id != userId && c.UserName != "admin"
                //            select new UserConversationVM
                //            {
                //                Id = c.Id,
                //                UserName = c.UserName,
                //                IsLogin = true,
                //                ImageUrl = c.ProfilePic,
                //                count = c.Conversations.Where(a => a.To == userId && a.IsSeen == false).Count()

                //            };

                //var query = from c in db.AspNetUsers
                //            where c.Id != null
                //            select new UserConversationVM
                //            {
                //                Id = c.Id,
                //                UserName = c.UserName,
                //                IsLogin = true,
                //                ImageUrl = c.ProfilePic,
                //                count = c.Conversations.Count()

                //            };
                return PartialView(query.ToList().OrderByDescending(x => x.Id));
            }

        }

        public ActionResult SendMessage()
        {
            string message = "";
            int From = Convert.ToInt32(Request.Form[""]);
            List<ConversationVM> result = new List<ConversationVM>();

            try
            {
                IFormFile file = (IFormFile)null;

                IFormFile files = Request.Form.Files[0];
                string fname = string.Empty;
                if (files.Length > 0)
                {
                    file = files;
                    string browser = _detection.Browser.Type.ToString() + _detection.Browser.Version;
                    if (browser.ToUpper() == "IE" || browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = file.FileName;
                    }
                }
                var Message = Request.Form[""];
                var chkMessage = Message;
                if (chkMessage.Contains("@") || chkMessage.Contains("gmail") || chkMessage.Contains("yahoo") || chkMessage.Contains("live") ||
                    chkMessage.Contains("hotmail") || chkMessage.Contains("facebook") || chkMessage.Contains("whatsapp") || chkMessage.Contains("address") || chkMessage.Contains("email")
                    || chkMessage.Contains("street") || chkMessage.Contains("house") || chkMessage.Contains("#") || chkMessage.Contains("no") || chkMessage.Contains("city") || chkMessage.Contains("country")
                    || chkMessage.Contains("contry") || chkMessage.Contains("skype") || chkMessage.Contains("imo") || chkMessage.Contains("twitter") || chkMessage.Contains("pay") || chkMessage.Contains("instagram")
                    || chkMessage.Contains("payment") || chkMessage.Contains("card") || chkMessage.Contains("cash") || chkMessage.Contains("location") || chkMessage.Contains("social") || chkMessage.Contains("personal")
                    || chkMessage.Contains("contact") || chkMessage.Contains("phone") || chkMessage.Contains("mobile"))
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    message = "Error";
                    return Json(new { data = message });
                }

                var To = Convert.ToInt32(1); // Convert.ToInt32(Request.Form[1]);
                var FileType = "png"; //Request.Form[2];


                Conversations obj = new Conversations();
                if (To != 0)
                {
                    obj.Text = Message;
                    obj.To = To;
                }
                int userId = GetLoggedInUserId();
                obj.Date = DateTime.Now;
                obj.From = userId;
                int count = 0;

                if (!string.IsNullOrWhiteSpace(fname))
                {
                    if (FileType == "Photo")
                    {
                        obj.Image = fname;
                    }

                    count++;
                }
                string status;
                using (_context = new ApplicationDbContext())
                {
                    try
                    {
                        _context.Conversation.Add(obj);
                        _context.SaveChanges();
                        status = "Ok";
                    }
                    catch (Exception)
                    {
                        status = "Error";
                        throw;
                    }

                }

                if (count == 1 && status == "Ok")
                {
                    //fname = System.IO.Path.Combine(Server.MapPath("~/Uploads/ChatImages"), fname);
                    //file.SaveAs(fname);
                }
                if (status == "Ok")
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    message = "Ok";
                }
                else
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    message = "Error";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                message = "Error";
            }

            using (_context = new ApplicationDbContext())
            {
                var UserId = GetLoggedInUserId();
                var results = _context.Conversation.Where(x => x.From == From && x.To == UserId || x.From == UserId && x.To == From && x.IsSeen == false).ToList();

                var query = from x in _context.Conversation
                            where x.From == From && x.To == UserId || x.From == UserId && x.To == From && x.IsSeen == false
                            select new ConversationVM
                            {
                                From = x.From,
                                IsSeen = x.IsSeen,
                                Text = x.Text,
                                Date = x.Date,
                                Image = x.Image

                            };

                foreach (var item in query)
                {
                    var sender = _context.Users.Where(x => x.Id == From).FirstOrDefault();
                    if (item.FromImage == null || item.FromImage.Length < 1)
                    {
                        item.FromImage = "Profileplaceholder.png";
                    }
                    else
                    {
                        item.FromImage = sender.Photo;
                    }
                    result.Add(item);

                }
                results.ToList();
                var data = message;
                return Json(data);

            }
        }

        public ActionResult GetConversation(int From)
        {
            try
            {
                List<ConversationVM> result = new List<ConversationVM>();
                using (_context = new ApplicationDbContext())
                {


                    var UserId = GetLoggedInUserId();
                    var UserPIC = _context.Users.Where(x => x.Id == UserId).FirstOrDefault();
                    //Session["UserDP"] = UserPIC.ProfilePic;

                    if (UserPIC.Photo != null || UserPIC.Photo.Length < 1)
                    {
                        ViewBag.ActiveUserpic = UserPIC.Photo;
                    }
                    else
                    {
                        ViewBag.ActiveUserpic = "Profileplaceholder.png";
                    }

                    var results = _context.Conversation.Where(x => x.From == From && x.To == UserId && x.IsSeen == false).ToList();

                    foreach (var x in results)
                    {
                        x.IsSeen = true;
                    }
                    _context.SaveChanges();

                    var query = from x in _context.Conversation
                                from u in _context.Users.Where(y => y.Id == x.From)
                                where x.From == UserId && x.To == From || x.From == From && x.To == UserId
                                select new ConversationVM
                                {
                                    From = x.From,
                                    IsSeen = x.IsSeen,
                                    Text = x.Text,
                                    Date = x.Date,
                                    Image = x.Image,
                                    FromImage = u.Photo,
                                };

                    foreach (var item in query)
                    {
                        var sender = _context.Users.Where(x => x.Id == item.From).FirstOrDefault();
                        //Session["UserDP"] = sender.ProfilePic;
                        item.FromImage = sender.Photo;

                        if (item.FromImage == null || item.FromImage.Length < 1)
                        {
                            item.FromImage = "Profileplaceholder.png";
                        }

                        result.Add(item);

                    }
                    //result = query.ToList();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(result);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json("Error in conversation");
            }

        }
        public ActionResult GetUnreadConversation(int From)
        {
            try
            {
                List<ConversationVM> result = new List<ConversationVM>();
                var UserId = GetLoggedInUserId();

                using (_context = new ApplicationDbContext())
                {
                   // _context.Configuration.ProxyCreationEnabled = false;

                    var results = _context.Conversation.Where(x => x.From == From && x.To == UserId && x.IsSeen == false).ToList();



                    var query = from x in _context.Conversation
                                where x.From == From && x.To == UserId && x.IsSeen == false
                                select new ConversationVM
                                {
                                    From = x.From,
                                    IsSeen = x.IsSeen,
                                    Text = x.Text,
                                    Date = x.Date,
                                    Image = x.Image

                                };

                    foreach (var item in query)
                    {
                        var sender = _context.Users.Where(x => x.Id == From).FirstOrDefault();
                        if (sender.Photo == null || sender.Photo.Length < 1)
                        {
                            item.FromImage = "Profileplaceholder.png";
                        }
                        else
                        {
                            item.FromImage = sender.Photo;
                        }
                        result.Add(item);

                    }


                    results.ToList();
                    foreach (var x in results)
                    {
                        x.IsSeen = true;
                    }
                    _context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(result);
            }
            catch (Exception exe)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json("Error in conversation");
            }

        }

        public ActionResult _Notification()
        {
            int userId = GetLoggedInUserId();
            using (_context = new ApplicationDbContext())
            {

                var query = _context.Conversation.Where(x => x.To == userId && x.IsSeen == false).Count();
                int notif = query;
                return PartialView(notif);
            }

        }

        public ActionResult GetLastestChat()
        {
            using (_context = new ApplicationDbContext())
            {

                try
                {
                    int userId = GetLoggedInUserId();

                    var query = from r in _context.Conversation
                                where r.To == userId && r.Text != null || r.From == userId
                                //orderby r.Date descending
                                group r by r.From into g
                                select g.OrderByDescending(x => x.Date).Take(5).FirstOrDefault();

                    List<LatestConversationVM> messages = new List<LatestConversationVM>();

                    foreach (var item in query)
                    {
                        LatestConversationVM NM = new LatestConversationVM();
                        var user = _context.Users.Find(item.From);
                        NM.From = item.From;
                        NM.FromName = user.FullName;
                        NM.Date = item.Date;
                        if (item.Text == "")
                        {
                            NM.Text = "New Image";
                        }
                        else
                        {
                            NM.Text = item.Text;
                        }

                        NM.Image = user.Photo;
                        if (NM.Image == null || NM.Image.Length < 1)
                        {
                            NM.Image = "Profileplaceholder.png";
                        }
                        messages.Add(NM);


                    }

                    Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    return Json(messages);

                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    return Json(new { data = "Error" });

                }
            }

        }
        public ActionResult Index2()
        {
            return View();
        }

    }
}
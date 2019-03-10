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

namespace TravelSystem.Controllers
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class MessagesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _hostingEnvironment;
        string machineName = GetIPAddress(Dns.GetHostName()).ToString();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public MessagesController(IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<MessagesController> logger, ApplicationDbContext context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
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
            using (db = new ApplicationDbContext())
            {
                var UserPIC = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();

                if (UserPIC.ProfilePic != null && UserPIC.ProfilePic.Length > 1)
                {

                    TempData["ActiveUserpic"] = UserPIC.ProfilePic;
                }
                else
                {
                    TempData["ActiveUserpic"] = "Profileplaceholder.png";
                }
            }

            if (!string.IsNullOrEmpty(id))
            {
                using (db = new ApplicationDbContext())
                {
                    AspNetUser U = db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
                    Conversation con = db.Conversations.Where(x => x.From == id || x.To == id).FirstOrDefault();
                    if (con == null)
                    {
                        Conversation dcon = new Conversation();
                        dcon.To = id;
                        dcon.From = userId;
                        dcon.IsSeen = true;
                        dcon.Date = DateTime.Now;
                        dcon.Text = "Now You Can Start Chat With Me";

                        db.Conversations.Add(dcon);
                        db.SaveChanges();
                    }

                    ViewBag.ActiveUserId = id;
                    ViewBag.ActiveUserName = U.FirstName + " " + U.LastName;
                    if (U.ProfilePic != null)
                    {
                        if (U.ProfilePic.Length < 1)
                        {
                            ViewBag.ActiveUserpic = U.ProfilePic;
                        }
                    }
                    if (U.ProfilePic == null)
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
            List<Conversation> addedUsr = new List<Conversation>();
            List<AspNetUser> refinedUsr = new List<AspNetUser>();
            List<UserConversationVM> query = new List<UserConversationVM>();
            string userId = User.Identity.GetUserId();
            using (db = new boothop2_DBEntities())
            {

                var results = db.Conversations.Where(x => x.From == userId || x.To == userId).ToList();
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
                    AspNetUser usr = db.AspNetUsers.Where(x => x.Id != userId && (x.Id == item.From || x.Id == item.To)).FirstOrDefault();
                    var Concount = db.Conversations.Where(a => a.To == userId && a.From == usr.Id && a.IsSeen == false);
                    int count = 0;
                    if (Concount != null)
                    {
                        count = Concount.Count();
                    }
                    UserConversationVM con = new UserConversationVM();
                    con.Id = usr.Id;
                    con.ImageUrl = usr.ProfilePic;
                    if (usr.IsLogin == null)
                    {
                        con.IsLogin = true;
                    }
                    else
                    {
                        con.IsLogin = (bool)usr.IsLogin;
                    }
                    con.UserName = usr.FirstName + " " + usr.LastName;
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
            string From = Request.Form[1];
            List<ConversationVM> result = new List<ConversationVM>();

            try
            {
                HttpPostedFileBase file = (HttpPostedFileBase)null;

                HttpFileCollectionBase files = Request.Files;
                string fname = string.Empty;
                if (files.Count > 0)
                {
                    file = files[0];

                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = file.FileName;
                    }
                }
                var Message = Request.Form[0];
                var chkMessage = Message.ToLower();
                if (chkMessage.Contains("@") || chkMessage.Contains("gmail") || chkMessage.Contains("yahoo") || chkMessage.Contains("live") ||
                    chkMessage.Contains("hotmail") || chkMessage.Contains("facebook") || chkMessage.Contains("whatsapp") || chkMessage.Contains("address") || chkMessage.Contains("email")
                    || chkMessage.Contains("street") || chkMessage.Contains("house") || chkMessage.Contains("#") || chkMessage.Contains("no") || chkMessage.Contains("city") || chkMessage.Contains("country")
                    || chkMessage.Contains("contry") || chkMessage.Contains("skype") || chkMessage.Contains("imo") || chkMessage.Contains("twitter") || chkMessage.Contains("pay") || chkMessage.Contains("instagram")
                    || chkMessage.Contains("payment") || chkMessage.Contains("card") || chkMessage.Contains("cash") || chkMessage.Contains("location") || chkMessage.Contains("social") || chkMessage.Contains("personal")
                    || chkMessage.Contains("contact") || chkMessage.Contains("phone") || chkMessage.Contains("mobile"))
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    message = "Error";
                    return Json(new { data = message }, JsonRequestBehavior.AllowGet);
                }

                var To = Request.Form[1];
                var FileType = Request.Form[2];


                Conversation obj = new Conversation();
                if (!string.IsNullOrWhiteSpace(To))
                {
                    obj.Text = Message;
                    obj.To = To;
                }
                string userId = User.Identity.GetUserId();
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
                using (db = new boothop2_DBEntities())
                {
                    try
                    {
                        db.Conversations.Add(obj);
                        db.SaveChanges();
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
                    fname = System.IO.Path.Combine(Server.MapPath("~/Uploads/ChatImages"), fname);
                    file.SaveAs(fname);
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

            //using (db = new boothop2_DBEntities())
            //{
            //    var UserId = User.Identity.GetUserId();
            //    var results = db.Conversations.Where(x => x.From == From && x.To == UserId ||  x.From == UserId && x.To == From && x.IsSeen == false).ToList();

            //    var query = from x in db.Conversations
            //                where x.From == From && x.To == UserId || x.From == UserId && x.To == From && x.IsSeen == false
            //                select new ConversationVM
            //                {
            //                    From = x.From,
            //                    IsSeen = x.IsSeen,
            //                    Text = x.Text,
            //                    Date = x.Date,
            //                    Image = x.Image

            //                };

            //    foreach (var item in query)
            //    {
            //        var sender = db.AspNetUsers.Where(x => x.Id == From).FirstOrDefault();
            //        if (item.FromImage == null || item.FromImage.Length < 1)
            //        {
            //            item.FromImage = "Profileplaceholder.png";
            //        }
            //        else
            //        {
            //            item.FromImage = sender.ProfilePic;
            //        }
            //        result.Add(item);

            //    }
            //    results.ToList();
            var data = message;
            return Json(data, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetConversation(string From)
        {
            try
            {
                List<ConversationVM> result = new List<ConversationVM>();
                using (db = new boothop2_DBEntities())
                {


                    var UserId = User.Identity.GetUserId();
                    var UserPIC = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();
                    //Session["UserDP"] = UserPIC.ProfilePic;

                    if (UserPIC.ProfilePic != null || UserPIC.ProfilePic.Length < 1)
                    {
                        ViewBag.ActiveUserpic = UserPIC.ProfilePic;
                    }
                    else
                    {
                        ViewBag.ActiveUserpic = "Profileplaceholder.png";
                    }

                    var results = db.Conversations.Where(x => x.From == From && x.To == UserId && x.IsSeen == false).ToList();

                    foreach (var x in results)
                    {
                        x.IsSeen = true;
                    }
                    db.SaveChanges();

                    var query = from x in db.Conversations
                                from u in db.AspNetUsers.Where(y => y.Id == x.From)
                                where x.From == UserId && x.To == From || x.From == From && x.To == UserId
                                select new ConversationVM
                                {
                                    From = x.From,
                                    IsSeen = x.IsSeen,
                                    Text = x.Text,
                                    Date = x.Date,
                                    Image = x.Image,
                                    FromImage = u.ProfilePic,

                                };

                    foreach (var item in query)
                    {
                        var sender = db.AspNetUsers.Where(x => x.Id == item.From).FirstOrDefault();
                        //Session["UserDP"] = sender.ProfilePic;
                        item.FromImage = sender.ProfilePic;

                        if (item.FromImage == null || item.FromImage.Length < 1)
                        {
                            item.FromImage = "Profileplaceholder.png";
                        }

                        result.Add(item);

                    }
                    //result = query.ToList();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json("Error in conversation", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetUnreadConversation(string From)
        {
            try
            {
                List<ConversationVM> result = new List<ConversationVM>();
                var UserId = User.Identity.GetUserId();

                using (db = new boothop2_DBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var results = db.Conversations.Where(x => x.From == From && x.To == UserId && x.IsSeen == false).ToList();



                    var query = from x in db.Conversations
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
                        var sender = db.AspNetUsers.Where(x => x.Id == From).FirstOrDefault();
                        if (sender.ProfilePic == null || sender.ProfilePic.Length < 1)
                        {
                            item.FromImage = "Profileplaceholder.png";
                        }
                        else
                        {
                            item.FromImage = sender.ProfilePic;
                        }
                        result.Add(item);

                    }


                    results.ToList();
                    foreach (var x in results)
                    {
                        x.IsSeen = true;
                    }
                    db.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exe)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json("Error in conversation", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult _Notification()
        {
            string userId = User.Identity.GetUserId();
            using (db = new boothop2_DBEntities())
            {

                var query = db.Conversations.Where(x => x.To == userId && x.IsSeen == false).Count();
                int notif = query;
                return PartialView(notif);
            }

        }

        public ActionResult GetLastestChat()
        {
            using (db = new boothop2_DBEntities())
            {

                try
                {
                    string userId = User.Identity.GetUserId();

                    var query = from r in db.Conversations
                                where r.To == userId && r.Text != null || r.From == userId
                                //orderby r.Date descending
                                group r by r.From into g
                                select g.OrderByDescending(x => x.Date).Take(5).FirstOrDefault();

                    List<LatestConversationVM> messages = new List<LatestConversationVM>();

                    foreach (var item in query)
                    {
                        LatestConversationVM NM = new LatestConversationVM();
                        AspNetUser user = db.AspNetUsers.Find(item.From);
                        NM.From = item.From;
                        NM.FromName = user.FirstName + " " + user.LastName;
                        NM.Date = item.Date;
                        if (item.Text == "")
                        {
                            NM.Text = "New Image";
                        }
                        else
                        {
                            NM.Text = item.Text;
                        }

                        NM.Image = user.ProfilePic;
                        if (NM.Image == null || NM.Image.Length < 1)
                        {
                            NM.Image = "Profileplaceholder.png";
                        }
                        messages.Add(NM);


                    }

                    Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    return Json(messages, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    return Json(new { data = "Error" }, JsonRequestBehavior.AllowGet);

                }
            }

        }
        public ActionResult Index2()
        {
            return View();
        }

    }
}
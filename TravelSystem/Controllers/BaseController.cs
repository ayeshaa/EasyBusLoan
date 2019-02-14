using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TravelSystem.Controllers
{
    //[SessionFilter]
    public class BaseController : Controller
    {
        public int GetLoggedInUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            return 0;
        }
        public string GetLoggedInUserName()
        {
            var userName = HttpContext.Session.GetString("UserName");
            return userName != null ? userName : null;
        }
        public static IPAddress GetIPAddress(string hostName)
        {
            Ping ping = new Ping();
            var replay = ping.Send(hostName);

            if (replay.Status == IPStatus.Success)
            {
                return replay.Address;
            }
            return null;
        }
    }
}
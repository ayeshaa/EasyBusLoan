using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;

namespace TravelSystem.Helpers
{
    public static class Html
    {
        public static bool IsLendor(this HtmlHelper html)
        {
            var user = html.ViewContext.HttpContext.User;
            return user.IsInRole("Lendor");
        }
        public static bool IsBuyer(this HtmlHelper html)
        {
            var user = html.ViewContext.HttpContext.User;
            return user.IsInRole("Buyer");
        }
        public static bool IsSeller(this HtmlHelper html)
        {
            var user = html.ViewContext.HttpContext.User;
            return user.IsInRole("Seller");
        }
        public static string RemoveHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelSystem.Controllers;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, int userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(UserController.ConfirmEmail),
                controller: "User",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, int userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(UserController.ResetPassword),
                controller: "User",
                values: new { userId, code },
                protocol: scheme);
        }
    }
}

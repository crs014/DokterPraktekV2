using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace DokterPraktekV2.Models
{
    public class LoginUserId
    {
        public static Guid getCurrentUserGUID()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                MembershipUser myObject;
                myObject = Membership.GetUser(HttpContext.Current.User.Identity.Name);
                return (Guid)myObject.ProviderUserKey;
            }
            return Guid.Empty;
        }
    }
}
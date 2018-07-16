using FinancialPlanner.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace FinancialPlanner.Extension_Methods
{
    public static class InvitationExtension
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        //public static async Task SendInvitation( this Invitation invitation)
        //{
        //    //compose email
        //    //var body = new StringBuilder();
        //    //body.AppendFormat("<p>Email From: <bold>{0}</bold>({1})</p>", WebConfigurationManager.AppSettings["emailfrom"]);
        //    //body.AppendLine("br/><p><u><b>Message: ")

        //    IdentityMessage email = null;
        //    var callbackUrl = Url("Register", "Account")
        //    email = new IdentityMessage()
        //    {
        //        Subject = "You have been invited to join {0}" houseName,
        //        Body = invitation.Body,
        //        Destination = invitation.Email
        //    };
        //}
    }
}
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;
using kayako_ls_ad_aspnet_mvc.Models;

namespace kayako_ls_ad_aspnet_mvc.Controllers
{
    public class AuthenticateController : Controller
    {
        private string strLoginShareXML = "";
        //
        // GET: /Authenticate/
        [AcceptVerbs("GET")]
        public ActionResult Index()
        {
            var options = new List<SelectListItem>();

            options.Add(new SelectListItem { Value = "", Text = "Null" });
            options.Add(new SelectListItem { Value = "staff", Text = "staff" });
            options.Add(new SelectListItem { Value = "admin", Text = "admin" });
            options.Add(new SelectListItem { Value = "winapp", Text = "winapp" });
            options.Add(new SelectListItem { Value = "mobile", Text = "mobile" });

            ViewData["interface"] = options;

            return View();
        }

        //
        // POST: /Authenticate/
        [AcceptVerbs("POST")]
        public string Index(FormCollection fcLoginShare)
        {
            try
            {
                strLoginShareXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
                strLoginShareXML += "<loginshare>\n";
                string[] ADDomainStrings = LoginShareADDomain.ADDomainInfo;
                LoginShareUser lsuser = new LoginShareUser { lsusername = fcLoginShare["username"], lspassword = fcLoginShare["password"], lsipaddress = fcLoginShare["ipaddress"] };
                if (!string.IsNullOrEmpty(fcLoginShare["interface"]))
                {
                    lsuser.lsinterface = fcLoginShare["interface"];
                }
                PrincipalContext context;
                UserPrincipal userPrincipal = null;
                foreach (string s in ADDomainStrings)
                {
                    if (userPrincipal == null)
                    {
                        context = new PrincipalContext(ContextType.Domain, s, lsuser.lsusername, lsuser.lspassword);
                        userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, lsuser.lsusername);
                    }
                }
                if (userPrincipal == null)
                {
                    strLoginShareXML += "    <result>0</result>\n";
                    strLoginShareXML += "    <message>Invalid Username or Password</message>\n";
                }
                else
                {
                    strLoginShareXML += "    <result>1</result>\n";
                    switch (lsuser.lsinterface)
                    {
                        case "staff":
                        case "admin":
                        case "winapp":
                        case "mobile":
                            {
                                strLoginShareXML += "    <staff>\n";
                                strLoginShareXML += "        <firstname>" + userPrincipal.GivenName + "</firstname>\n";
                                strLoginShareXML += "        <lastname>" + userPrincipal.Surname + "</lastname>\n";
                                strLoginShareXML += "        <designation></designation>\n";
                                strLoginShareXML += "        <email>" + userPrincipal.EmailAddress + "</email>\n";
                                strLoginShareXML += "        <mobilenumber></mobilenumber>\n";
                                strLoginShareXML += "        <signature></signature>\n";
                                strLoginShareXML += "        <team>Staff</team>\n";
                                strLoginShareXML += "    </staff>\n";
                                break;
                            }
                        default:
                            {
                                strLoginShareXML += "    <user>\n";
                                strLoginShareXML += "        <usergroup>Registered</usergroup>\n";
                                strLoginShareXML += "        <fullname>" + userPrincipal.GivenName + " " + userPrincipal.Surname + "</fullname>\n";
                                /*
                                 * The following fields are optional.
                                 * strLoginShareXML += "        <designation></designation> <!-- Optional -->\n";
                                 * strLoginShareXML += "        <organization></organization> <!-- Optional: If you wish to specify the organization for user, this is only applicable for new users -->\n";
                                 * strLoginShareXML += "        <organizationtype></organizationtype> <!-- Optional: Can be \"shared\" or \"restricted\" (default) -->\n";
                                 */
                                strLoginShareXML += "        <emails>\n";
                                strLoginShareXML += "            <email>" + userPrincipal.EmailAddress + "</email>\n";
                                strLoginShareXML += "        </emails>\n";
                                strLoginShareXML += "        <phone>" + userPrincipal.VoiceTelephoneNumber + "</phone> <!-- Optional -->\n";
                                strLoginShareXML += "    </user>\n";
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                strLoginShareXML += "    <result>0</result>\n";
                strLoginShareXML += "    <message>" + ex.Message + "</message>\n";
            }
            strLoginShareXML += "</loginshare>\n";
            return strLoginShareXML;
        }

    }
}

using System.Configuration;
using System.Linq;

namespace kayako_ls_ad_aspnet_mvc.Models
{
    public class LoginShareADDomain
    {
        public static string[] ADDomainInfo
        {
            get
            {
                string[] ADDomainStrings = ConfigurationManager.AppSettings["ADDomainStrings"].Split(',').Select(s => s.Trim()).ToArray();
                return ADDomainStrings;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.External_Logins
{
   public class ExternalLoginAuth
{
    public FacebookSettings Facebook { get; set; }
    public GoogleSettings Google { get; set; }

    public class FacebookSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class GoogleSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}

}

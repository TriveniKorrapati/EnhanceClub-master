using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace EnhanceClub.WebUI.AwsHelper
{
    public class RdsConnection
    {
        public string username { get; set; }
        public string password { get; set; }
        public string engine { get; set; }
        public string dbInstanceIdentifier { get; set; }
        public string host { get; set; }

        public string ConString
        {
            get
            {
                StringBuilder sbCon = new StringBuilder();
                sbCon.Append("Data Source=");
                sbCon.Append(host);
                sbCon.Append(";");
                sbCon.Append("Initial Catalog=");
                //  sbCon.Append(dbInstanceIdentifier); // un comment after Michael F fixes in generation of secret
                sbCon.Append("vmo"); // comment after above is uncommented
                sbCon.Append(";");
                sbCon.Append("User ID=");
                sbCon.Append(username);
                sbCon.Append(";");
                sbCon.Append("Password=");
                sbCon.Append(password);
                sbCon.Append(";");
                sbCon.Append("Connect Timeout=45");
                return sbCon.ToString();
            }
        }
    }
}
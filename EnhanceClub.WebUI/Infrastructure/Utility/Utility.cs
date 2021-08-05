using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace EnhanceClub.WebUI.Infrastructure.Utility
{
    public static class Utility
    {
        // convert anonymous objects to expando objects
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = new RouteValueDictionary(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentException();
            }
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }

        public static string UrlControllerAction()
        {
            // get current url in controller.action format
            string contName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            string actName = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            return contName + "." + actName;
        }

        // this method is used to remove page query string from url
        public static string UriWithoutPagination(string originalUrl, string removeParam)
        {
            return originalUrl.Replace(removeParam, "");

        }

        public static string ViewNameFromViewPath(string viewPath)
        {
            var thisString = viewPath.Split('/');

            var elementCount = thisString.Length;

            var viewFileName = thisString[elementCount - 1];

            var splitFile = viewFileName.Split('.');

            return splitFile[0].ToLower();
        }

        // comment: #AddVisitorIpAddress: get visitor ip address
        public static string GetVisitorIpAddress()
        {
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        public static string DecryptCfm(string keyToDecrypt, string encryptedString)
        {

            byte[] key = Convert.FromBase64String(keyToDecrypt);
            byte[] data = Convert.FromBase64String(encryptedString);

            using (RijndaelManaged algorithm = new RijndaelManaged())
            {

                // initialize settings to match those used by CF
                algorithm.Mode = CipherMode.ECB;
                algorithm.Padding = PaddingMode.PKCS7;
                algorithm.BlockSize = 128;
                algorithm.KeySize = 128;
                algorithm.Key = key;


                ICryptoTransform decryptor = algorithm.CreateDecryptor();

                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (CryptoStream cs = new CryptoStream(ms,
                        decryptor,
                        CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }

        }

        // encrypt using format that can be read by cold fusion 
        public static string EncryptCfm(string keyToEncrypt, string stringToEncrypt)
        {

            byte[] key = Convert.FromBase64String(keyToEncrypt);
            byte[] encrypted;
            using (RijndaelManaged algorithm = new RijndaelManaged())
            {

                // initialize settings to match those used by CF
                algorithm.Mode = CipherMode.ECB;
                algorithm.Padding = PaddingMode.PKCS7;
                algorithm.BlockSize = 128;
                algorithm.KeySize = 128;
                algorithm.Key = key;

                ICryptoTransform encryptor = algorithm.CreateEncryptor();

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(ms,
                        encryptor,
                        CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(stringToEncrypt);
                        }

                        encrypted = ms.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string StripGmailAddress(string email)
        {
            var cleanEmailAddress = email;

            var splittedEmail = email.Split('@');
            var emailString = splittedEmail[0];
            var emailDomain = splittedEmail[1];
            if (emailDomain == "gmail.com" || emailDomain == "googlemail.com")
            {
                if (emailString.Contains("+"))
                {
                    emailString = emailString.Split('+')[0];
                }
                if (emailString.Contains("."))
                {
                    emailString = emailString.Replace(".", "");
                }

                cleanEmailAddress = emailString + "@" + emailDomain;
            }


            return cleanEmailAddress;
        }

        public static Dictionary<string, string> ConvertStringToDictionary(string paramList, char splitChar)
        {
            var keyValues = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(paramList))
            {
                var paramArray = paramList.Split(splitChar);

                foreach (var s in paramArray)
                {
                    keyValues.Add(s.Split('=')[0], s.Split('=')[1]);
                }
            }
            return keyValues;
        }
        public static string StripSpecialChar(string strToStrip)
        {
            // Remove special characters from first name
            if (!string.IsNullOrEmpty(strToStrip))
            {
                strToStrip = strToStrip.Replace("'", "")
                 .Replace(">", "")
                 .Replace("win.ini", "")
                 .Replace("boot.ini", "")
                 .Replace("convert", "")
                 .Replace("varchar", "")
                 .Replace(" or ", "")
                 .Replace(" and ", "")
                 .Replace("*", "")
                 .Replace("%", "")
                 .Replace("ping", "")
                 .Replace("sleep", "")
                 .Replace("waitfor", "")
                 .Replace("script", "")
                 .Replace("1=1", "")
                 .Replace("0=0", "")
                 .Replace("<", "")
                 .Replace(":", "")
                 .Replace(";", "")
                 .Replace("=", "")
                 .Replace("\\", "")
                 .Replace("//", "")
                 .Replace("(", "")
                 .Replace(")", "")
                 .Replace("''", "")
                 .Replace(@"""", "");
            } 

            return strToStrip;
         }

        public static bool InjectionPossible(string strToStrip)
        {

            bool injectionPossible = false;

            if (strToStrip.Contains("'"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains(">"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("<"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("win.ini"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("boot.ini"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("convert"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("varchar"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains(" or "))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains(" and "))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("*"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("%"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("ping"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("waitfor"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("sleep"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("script"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("1=1"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("="))
            {
                injectionPossible = true;
            }
            if (strToStrip.Contains(":"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains(";"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("''"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("/"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("int"))
            {
                injectionPossible = true;
            }

            if (strToStrip.Contains("char("))
            {
                injectionPossible = true;
            }
            return injectionPossible;
        }

        public static string GetSha1(string value)
        {
            var data = Encoding.ASCII.GetBytes(value);
            var hashData = new SHA1Managed().ComputeHash(data);
            var hash = string.Empty;
            return string.Join("", hashData.Select(b => string.Format("{0:X2}",
         b)).ToArray()).ToUpper();
        }
    }
}
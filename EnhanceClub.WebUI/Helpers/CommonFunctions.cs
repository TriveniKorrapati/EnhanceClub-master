using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EnhanceClub.WebUI.Helpers
{
    public static class CommonFunctions
    {
        // this function is used to generate pagination links, we pass page number and it gives start and end numbers to be shown
        // this function can be used to find start and end boundaries based on group size
        public struct NumberRange
        {
            public int NumberToFindGroup;  // this is input number for which we need group start and end
            public int GroupSize;          // this is input number that determines size of the group fo e.g 5 
            public int MaxGroup;        // this is input that contains max groups
            public int GroupStart;         // this is output that gives start number for group
            public int GroupEnd;           // this is output that gives end number of the group

            // calculate and set groupStart and groupEnd
            public void FindGroup()
            {

                int multiplier = NumberToFindGroup / GroupSize;

                if (NumberToFindGroup <= GroupSize)
                {
                    GroupStart = 1;
                }
                else
                {
                    if (NumberToFindGroup % GroupSize == 0)
                    {

                        GroupStart = (multiplier * GroupSize) - GroupSize + 1;
                    }
                    else
                    {
                        GroupStart = multiplier * GroupSize + 1;
                    }

                }

                GroupEnd = Math.Min(GroupStart + GroupSize - 1, MaxGroup);
            }
        }


        // these functions are used by news letter signup code to encrypt promo id for un subscribe link
        // BlogNewsLetterSignup Action in customer controller

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
               

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                encryptor.Padding = PaddingMode.Zeros;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.FlushFinalBlock();
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText.Replace('+', '-').Replace('/', '_'); 
        }

        public static string Decrypt(string cipherText)
        {
            //cipherText = cipherText.Replace('-','+').Replace('_','/');

            string cText = cipherText.Replace('-', '+').Replace('_', '/');
            var uncodedText = "";
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cText);
            using (Aes encryptor = Aes.Create())
            {
             
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                encryptor.Padding = PaddingMode.Zeros;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    //cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    uncodedText = Encoding.Unicode.GetString(ms.ToArray());
                    
                }
            }
            return uncodedText;
        }

        // adds line breaks to string, used to fix long product bname in related but
        public static string AddLineBreakByLength(string originalString, int atLen)
        {
            if (atLen < originalString.Length)
            {
                // get string after length passed
                int firstSpaceAt = originalString.IndexOf(" ", atLen, StringComparison.Ordinal);

                if (firstSpaceAt == -1)
                    return originalString;

                string newStringWithLineBreak = originalString.Remove(firstSpaceAt, 1).Insert(firstSpaceAt, " <br/>");

                return newStringWithLineBreak;    
            }
            else
            {
                return originalString;
            }

        }

        // remove last character from string, used to remove trailing / from request.Url for canonical
        public static string RemoveLastChar(string inputString)
        {
            var newString = inputString.Substring(0,inputString.Length - 1);
            return newString;
        }

        // comment: get user IpAddress
        public static string GetVisitorIpAddress()
        {
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        // get a list of original and changed property name
        public static List<string> GetChangedPropertiesName(object firstObject,
                                object secondObject)
        {
            List<string> changedProperties = new List<string>();

            if (firstObject.GetType() != secondObject.GetType())
            {
                throw new System.InvalidOperationException("Objects of different Type");
            }
            var propFirst = firstObject.GetType().GetProperties();


            foreach (PropertyInfo info in propFirst)
            {
                var propValueFirst = info.GetValue(firstObject,
                    null);
                var propValueSecond = info.GetValue(secondObject,
                    null);
                if (propValueFirst != null && propValueSecond != null &&
                    propValueFirst.ToString() != propValueSecond.ToString() &&
                    !info.Name.ToLower().Contains("lastmodified"))
                {
                    changedProperties.Add(info.Name);
                }

                if (propValueFirst == null && propValueSecond != null)
                {
                    changedProperties.Add(info.Name);
                }
            }


            return changedProperties;
        }
    }
}
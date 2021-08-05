using System;
using System.Text.RegularExpressions;

namespace EnhanceClub.Domain.Helpers
{
    public class CommonFunctions
    {
        // strip special characters to prevent sql injection
        public static string StripSpecialChar(string stripString)
        {
            if (!string.IsNullOrEmpty(stripString))
            {
                //  Regex.Replace(text, "hello", "<span>$&</span>", RegexOptions.IgnoreCase);
                stripString = stripString.Replace("'", "");
                stripString = stripString.Replace(">", "");
                stripString = stripString.Replace("<", "");
                stripString = stripString.Replace(":", "");
                stripString = stripString.Replace(";", "");
                stripString = stripString.Replace("=", "");
                stripString = stripString.Replace("\\", "");
                stripString = stripString.Replace("//", "");
                stripString = stripString.Replace("(", "");
                stripString = stripString.Replace(")", "");
                stripString = stripString.Replace("''", "");
                stripString = stripString.Replace(@"""", "");
                stripString = stripString.Replace(@"--", "");
                stripString = stripString.Replace(@"'", "");
                stripString = Regex.Replace(stripString, @"select", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"order by", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"dbms", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @" and ", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"union all", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"waitfor", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"sleep", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"delay", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"cast", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"case", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @" for ", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"upper", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"update", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"delete", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"insert", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"join", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"union", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"drop", "", RegexOptions.IgnoreCase);
                stripString = Regex.Replace(stripString, @"truncate", "", RegexOptions.IgnoreCase);
            }
            return stripString;
        }
      
        // function to handle apostrophe symbol
        public static string StripApostropheSymbol(String apostropheString)
        {
            if (!string.IsNullOrEmpty(apostropheString))
            {
                apostropheString = apostropheString.Replace("'", "''");
            }
            return apostropheString;

        }
    }
}

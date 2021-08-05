using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.HtmlHelpers
{
    public static class ExtensionsTimeZone
    {
        // method handle date property that accept null value
        public static string ToClientTime(this DateTime? entryTime, bool isOnlyDate = true, string dateFormat = "dd-MMM-yyyy")
        {
            var timeOffSet = HttpContext.Current.Session["UserTimeZoneOffsetMinutes"];

            if (timeOffSet != null && entryTime != null)
            {
                DateTime _entryTime = Convert.ToDateTime(entryTime);
                var offset = int.Parse(timeOffSet.ToString());
                _entryTime = _entryTime.AddMinutes(-1 * offset);

                if (isOnlyDate)
                {
                    return _entryTime.ToString(dateFormat);
                }

                return _entryTime.ToString(dateFormat);
            }

            // If there is no offset in session then return datetime in server timezone
            return entryTime.ToString();
        }

        public static string ToClientTime(this DateTime entryTime, bool isOnlyDate = true, string dateFormat = "dd-MMM-yyyy")
        {
            var timeOffSet = HttpContext.Current.Session["UserTimeZoneOffsetMinutes"];

            if (timeOffSet != null && entryTime != null)
            {
                var offset = int.Parse(timeOffSet.ToString());
                entryTime = entryTime.AddMinutes(-1 * offset);

                if (isOnlyDate)
                {
                    return entryTime.ToString(dateFormat);
                }

                return entryTime.ToString(dateFormat);
            }

            // If there is no offset in session then return datetime in server timezone
            return entryTime.ToString();
        }
    }
}
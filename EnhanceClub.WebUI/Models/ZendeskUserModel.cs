using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.Models
{
    public class ZendeskUserModel
    {
        public ZendeskUser user { get; set; }
    }

    public class ZendeskUser
    {
        public long? id { get; set; }
        public string phone { get; set; }

        //public string url { get; set; }
        //public string name { get; set; }
        //public string email { get; set; }
        //public DateTime created_at { get; set; }
        //public DateTime updated_at { get; set; }
        //public string time_zone { get; set; }
        //public string iana_time_zone { get; set; }   
        //public bool shared_phone_number { get; set; }
        //public object photo { get; set; }
        //public int locale_id { get; set; }
        //public string locale { get; set; }
        //public object organization_id { get; set; }
        //public string role { get; set; }
        //public bool verified { get; set; }
        //public string authenticity_token { get; set; }
    } 
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.Models
{
    public class ZendeskTicketModel
    {
        public ZendeskRequest request { get; set; }
    }

    public class ZendeskCustomField
    {
        public long id { get; set; }
        public string value { get; set; }
    }

    public class ZendeskRequester
    {
        public string name { get; set; }
        public string email { get; set; }
    }

    public class ZendeskComment
    {
        public string body { get; set; }
    }

    public class ZendeskRequest
    {
        public ZendeskRequester requester { get; set; }
        public string subject { get; set; }
        public ZendeskComment comment { get; set; }
        public List<ZendeskCustomField> custom_fields { get; set; }
    }

    public class ZendeskTicketResponse
    {
        public ZendeskRequestReponse request { get; set; }
    }

    public class ZendeskRequestReponse
    {
        public string url { get; set; }
        public int id { get; set; }
        public string status { get; set; }
    }
}
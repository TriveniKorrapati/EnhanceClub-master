using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.Models
{
    public class ZendeskCategoryModel
    {
        public TicketField ticket_field { get; set; }
    }

    public class CustomFieldOption
    {
        public object id { get; set; }
        public string name { get; set; }
        public string raw_name { get; set; }
        public string value { get; set; }
        public bool @default { get; set; }
    }

    public class TicketField
    {
        public string url { get; set; }
        public long id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string raw_title { get; set; }
        public string description { get; set; }
        public string raw_description { get; set; }
        public int position { get; set; }
        public bool active { get; set; }
        public bool required { get; set; }
        public bool collapsed_for_agents { get; set; }
        public object regexp_for_validation { get; set; }
        public string title_in_portal { get; set; }
        public string raw_title_in_portal { get; set; }
        public bool visible_in_portal { get; set; }
        public bool editable_in_portal { get; set; }
        public bool required_in_portal { get; set; }
        public object tag { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool removable { get; set; }
        public object agent_description { get; set; }
        public List<CustomFieldOption> custom_field_options { get; set; }
    } 
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.Models
{
    public class GetInTouchViewModel
    {
        public string CaptchaSiteKey { get; set; }

        public List<CustomFieldOption> custom_field_options { get; set; }
    }
}
namespace EnhanceClub.WebUI.Models
{
    public class BreadCrumbLink
    {
        public string LinkText { get; set; }
        public string LinkUrl { get; set; }
        public int TargetWindow { get; set; }
        public bool MakeLink { get; set; }
        public bool IsActive { get; set; }
        public string BaseUrl { get; set; }
    }
}
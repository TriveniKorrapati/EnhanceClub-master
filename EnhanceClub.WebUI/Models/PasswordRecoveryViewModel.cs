using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    public class PasswordRecoveryViewModel
    {
        public CustomerLoginInfo CustomerLoginInfo { get; set; }
        public string EmailProvided { get; set; }
    }
}
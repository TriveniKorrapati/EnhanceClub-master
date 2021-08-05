using System.ComponentModel.DataAnnotations;

namespace EnhanceClub.WebUI.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public int CustomerId { get; set; }

        public bool Terms { get; set; }

        public bool TeleHealthTerms { get; set; }

    }
}
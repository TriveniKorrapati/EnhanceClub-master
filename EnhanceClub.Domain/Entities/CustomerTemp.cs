using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EnhanceClub.Domain.Entities
{
    public class CustomerTemp
    {
        [Required(ErrorMessage = "Please Enter the First Name")]
        public string CustomerFirstName { get; set; }

        [Required(ErrorMessage = "Please Enter the Last Name")]
        public string CustomerLastName { get; set; }

        [Required]
        [Remote("DoesEmailExist", "Customer", "test")]
        public string CustomerEmail { get; set; }
    }
}

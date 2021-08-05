namespace EnhanceClub.Domain.Entities
{
    // used for password recovery
    public class CustomerLoginInfo
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public int CustomerId { get; set; }

        public string CustomerLastIp { get; set; }

        public string Token { get; set; }

        public string FirstName  { get; set; }

        public bool SignupEmailVerified { get; set; }
    }
}

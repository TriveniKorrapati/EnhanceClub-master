
namespace EnhanceClub.Domain.Abstract
{
    public interface IAuthProvider
    {
        bool Authenticate(string email, string password, ICustomerRepository repository);

        bool PreAuthenticate(string email, ICustomerRepository repository);
    }
}

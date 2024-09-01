using Onion.Application.Bases;

namespace Onion.Application.Features.Auth.Exceptions
{
    public class EmailOrPasswordShouldNotBeInvalidException : BaseException
    {
        public EmailOrPasswordShouldNotBeInvalidException(): base("Kullanıcı ad ıveya şifre hatalı")
        {
            
        }
    }
}

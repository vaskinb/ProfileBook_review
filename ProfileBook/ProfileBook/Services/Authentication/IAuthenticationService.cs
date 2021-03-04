using System.Threading.Tasks;
using ProfileBook.Models;

namespace ProfileBook.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<AOResult<UserModel>> SignUpAsync(string login, string password);
        Task<AOResult> SignInAsync(string login, string password);
    }
}

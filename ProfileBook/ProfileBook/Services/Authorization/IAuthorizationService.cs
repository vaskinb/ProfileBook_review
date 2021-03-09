
namespace ProfileBook.Services.Authorization
{
    public interface IAuthorizationService
    {
        bool IsAuthorized { get; }
        void Unauthorize();
        int GetCurrenUserId();
    }
}

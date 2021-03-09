using System;
using System.Linq;
using System.Threading.Tasks;
using ProfileBook.Models;
using ProfileBook.Services.Authorization;
using ProfileBook.Services.Repository;
using ProfileBook.Services.Settings;

namespace ProfileBook.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository _repository;
        private readonly ISettingsManager _settingsManager;

        public AuthenticationService(IRepository repository,
            IAuthorizationService authorizationService,
            ISettingsManager settingsManager)
        {
            _repository = repository;
            _settingsManager = settingsManager;
        }

        #region -- IAuthenticationService implementation --

        public async Task<AOResult<UserModel>> SignUpAsync(string login, string password)
        {
            var result = new AOResult<UserModel>();
            var savingUser = new UserModel()
            {
                Login = login,
                Password = password
            };

            try
            {
                var res = await _repository.SaveAsync(savingUser);
                if (res == 1)
                {
                    result.SetSuccess(savingUser);
                }
            }
            catch (Exception ex)
            {
                result.SetFailure();
            }
            return result;
        }

        public async Task<AOResult> SignInAsync(string login, string password)
        {
            var result = new AOResult();
            var res = await _repository.FindByAsync<UserModel>(x => x.Login == login);

            if (res.Count > 0)
            {
                var userModel = res.FirstOrDefault();

                if (userModel.Password == password)
                {
                    _settingsManager.AuthorizedUserID = (int)userModel.Id;
                    result.SetSuccess();
                }
                else
                {
                    result.SetFailure();
                }
            }
            return result;
        }
        #endregion
    }
}

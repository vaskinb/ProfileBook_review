using System;
using ProfileBook.Services.Settings;

namespace ProfileBook.Services.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ISettingsManager _settingsManager;

        public AuthorizationService(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        #region ---  Implementations ---
        public bool IsAuthorized
        {
            get => _settingsManager.AuthorizedUserID >= 0;
        }

        public int GetCurrenUserId()
        {
            return _settingsManager.AuthorizedUserID;
        }

        public void Unauthorize()
        {
            _settingsManager.AuthorizedUserID = -1;
        }

        #endregion
    }
}

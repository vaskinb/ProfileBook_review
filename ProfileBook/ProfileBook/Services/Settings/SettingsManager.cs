using System;
using Plugin.Settings.Abstractions;

namespace ProfileBook.Services.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private readonly ISettings _appSettings;

        public SettingsManager(
            ISettings appSettings)
        {
            _appSettings = appSettings;
        }

        #region -- ISettingsManager implementation --

        public int AuthorizedUserID
        {
            get => _appSettings.GetValueOrDefault(nameof(AuthorizedUserID), -1);
            set => _appSettings.AddOrUpdateValue(nameof(AuthorizedUserID), value);
        }

        public void ClearData()
        {
            AuthorizedUserID = -1;
        }

        #endregion
    }
}
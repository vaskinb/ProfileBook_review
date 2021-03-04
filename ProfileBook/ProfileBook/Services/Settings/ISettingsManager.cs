using System;

namespace ProfileBook.Services.Settings
{
    public interface ISettingsManager
    {
        int AuthorizedUserID { get; set; }
    }
}

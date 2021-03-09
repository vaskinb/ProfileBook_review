using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProfileBook.Models;

namespace ProfileBook.Services.Profile
{
    public interface IProfileService
    {
        Task<AOResult<List<ProfileModel>>> GetProfileListAsync();
        Task<AOResult> SaveOrUpdateProfileToStorageAsync(ProfileModel profileModel);
        Task<AOResult> SaveProfileToStorageAsync(ProfileModel profileModel);
        Task<AOResult> DeleteProfileFromStorageAsync(ProfileModel profileModel);
    }
}

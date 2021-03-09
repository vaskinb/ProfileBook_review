using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProfileBook.Models;
using ProfileBook.Services.Authorization;
using ProfileBook.Services.Repository;

namespace ProfileBook.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly IRepository _repository;
        private readonly IAuthorizationService _authorizationService;

        public ProfileService(
            IRepository repository,
            IAuthorizationService authorizationService)
        {
            _repository = repository;
            _authorizationService = authorizationService;
        }

        #region --- Implementations ---

        public async Task<AOResult> DeleteProfileFromStorageAsync(ProfileModel profileModel)
        {
            var result = new AOResult();

            var deleteRes = await _repository.DeleteAsync(profileModel);

            if (deleteRes == 1)
            {
                result.SetSuccess();
            }

            return result;
        }

        public async Task<AOResult<List<ProfileModel>>> GetProfileListAsync()
        {
            var result = new AOResult<List<ProfileModel>>();
            var userId = _authorizationService.GetCurrenUserId();
            var list = await _repository.FindByAsync<ProfileModel>((System.Linq.Expressions.Expression<Func<ProfileModel, bool>>)(x => x.user_id == userId));

            if (list.Count > 0)
            {
                result.SetSuccess(list);
            }

            return result;
        }

        public async Task<AOResult> SaveOrUpdateProfileToStorageAsync(ProfileModel profileModel)
        {
            var result = new AOResult();

            try
            {
                await _repository.SaveOrUpdateAsync(profileModel);

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetFailure();
            }

            return result;
        }

        public async Task<AOResult> SaveProfileToStorageAsync(ProfileModel profileModel)
        {
            var result = new AOResult();

            try
            {
                await _repository.SaveAsync(profileModel);

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetFailure();
            }
            return result;
        }

        #endregion
    }
}

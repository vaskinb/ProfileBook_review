using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Prism.Navigation;
using ProfileBook.Models;
using ProfileBook.Services.Authorization;
using ProfileBook.Services.Profile;
using ProfileBook.Views;
using Xamarin.Forms;

namespace ProfileBook.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IProfileService _profileService;

        public MainPageViewModel(
            INavigationService navigationService,
            IUserDialogs userDialogs,
            IAuthorizationService authorizationService,
            IProfileService profileService
            )
            : base(navigationService, userDialogs)
        {
            _authorizationService = authorizationService;
            _profileService = profileService;
        }

        #region -- Public properties --

        private ObservableCollection<ProfileModel> _profiles;
        public ObservableCollection<ProfileModel> Profiles
        {
            get => _profiles;
            set => SetProperty(ref _profiles, value);
        }

        private bool _isProfilesAdded;
        public bool IsProfilesAdded
        {
            get => _isProfilesAdded;
            set => SetProperty(ref _isProfilesAdded, value);
        }

        public ICommand LogOutCommand => new Command(OnLogOutCommand);

        public ICommand SettingsCommand => new Command(OnSettingsCommand);

        public ICommand AddTappedCommand => new Command(OnAddTapedCommand);

        public ICommand DeleteCommand => new Command(OnDeleteCommandAsync);

        public ICommand EditCommand => new Command(OnEditCommandAsync);

        #endregion

        #region -- Overrides --

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            await LoadProfileListAsync();
        }

        #endregion

        #region -- Command Handlers --

        private async void OnLogOutCommand(object obj)
        {
            _authorizationService.Unauthorize();

            await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(SignIn)}");
        }

        private void OnSettingsCommand(object obj)
        {
            UserDialogs.Alert("Settings Page Not Implemented!!!!");
        }

        private async void OnAddTapedCommand(object obj)
        {
            await NavigationService.NavigateAsync($"{nameof(AddEditProfile)}");
        }

        private async void OnDeleteCommandAsync(object obj)
        {
            var confirmConfig = new ConfirmConfig()
            {
                Message = "DeleteConfirmMessage",
                OkText = "Delete",
                CancelText = "Cancel"
            };

            var confirm = await UserDialogs.ConfirmAsync(confirmConfig);

            if (confirm)
            {
                var result = await _profileService.DeleteProfileFromStorageAsync((ProfileModel)obj);

                if (result.IsSuccess)
                {
                    Profiles.Remove((ProfileModel)obj);

                    IsProfilesAdded = Profiles.Count < 1;
                }
            }
        }

        private async void OnEditCommandAsync(object obj)
        {
            var parameters = new NavigationParameters();
            parameters.Add(nameof(ProfileModel), (ProfileModel)obj);

            await NavigationService.NavigateAsync($"{nameof(AddEditProfile)}", parameters);
        }

        #endregion

        #region -- Private Helpers --

        private async Task LoadProfileListAsync()
        {
            var profileList = await _profileService.GetProfileListAsync();

            if (profileList.IsSuccess)
            {
                Profiles = new ObservableCollection<ProfileModel>(profileList.Result);

                IsProfilesAdded = Profiles.Count < 1;
            }
        }

        #endregion
    }
}

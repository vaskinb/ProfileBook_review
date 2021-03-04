using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Navigation;
using ProfileBook.Enums;
using ProfileBook.Models;
using ProfileBook.Services.Authorization;
using ProfileBook.Services.Profile;
using Xamarin.Forms;

namespace ProfileBook.ViewModels
{
    public class AddEditProfileViewModel : BaseViewModel
    {
        private readonly IProfileService _profileService;
        private readonly IAuthorizationService _authorizationService;

        public AddEditProfileViewModel(
            INavigationService navigationService,
            IUserDialogs userDialogs,
            IProfileService profileService,
            IAuthorizationService authorizationService)
            : base(navigationService, userDialogs)
        {
            _profileService = profileService;
            _authorizationService = authorizationService;
        }

        #region -- Public properties --

        private ProfileModel _profile;
        public ProfileModel Profile
        {
            get => _profile;
            set => SetProperty(ref _profile, value);
        }

        private string _nickName;
        public string NickName
        {
            get => _nickName;
            set => SetProperty(ref _nickName, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        private bool _canSave;
        public bool CanSave
        {
            get => _canSave;
            set => SetProperty(ref _canSave, value);
        }


        private string _imageSource = "no image";
        public string ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public ICommand SaveCommand => new Command(OnSaveCommandAsync);

        public ICommand ImageTapCommand => new Command(OnImageTapCommandAsync);

        #endregion

        #region -- Overrides --


        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(Profile))
            {
                NickName = Profile.nick_name;
                Name = Profile.name;
                Description = Profile.description;
                ImageSource = string.IsNullOrEmpty(Profile.image_path) ? "no image" : Profile.image_path;
            }
        }

        #endregion

        #region -- Command Handlers --

        private async void OnSaveCommandAsync()
        {

        }

        private void OnImageTapCommandAsync()
        {
            var cfg = new ActionSheetConfig()
                .SetTitle("Chose action")
                .Add("Camera", () => OpenCamera(), "ic_camera_alt")
                .Add("Gallery", () => PickPhoto(), "ic_collections")
                .SetCancel();

            UserDialogs.ActionSheet(cfg);
        }

        #endregion

        #region -- Private Helpers --

        private async void PickPhoto()
        {
            try
            {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    MediaFile image = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions());

                    if (image != null)
                    {
                        ImageSource = image.Path;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void OpenCamera()
        {
            try
            {
                if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                {
                    MediaFile image = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,
                        SaveToAlbum = true,
                        Directory = "Sample",
                        Name = $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.jpg"
                    });

                    if (image != null)
                    {
                        ImageSource = image.Path;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private bool FieldsValid()
        {
            return !string.IsNullOrEmpty(NickName) && !string.IsNullOrEmpty(Name);
        }

        #endregion
    }
}

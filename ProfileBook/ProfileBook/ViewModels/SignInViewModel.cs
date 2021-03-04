using System;
using System.ComponentModel;
using System.Windows.Input;
using Acr.UserDialogs;
using Prism.Navigation;
using ProfileBook.Models;
//using ProfileBook.Resources.Strings;
using ProfileBook.Services.Authentication;
using ProfileBook.Views;
using Xamarin.Forms;

namespace ProfileBook.ViewModels
{
    public class SignInViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;

        public SignInViewModel(
            INavigationService navigationService,
            IUserDialogs userDialogs,
            IAuthenticationService authenticationService)
            : base(navigationService, userDialogs)
        {
            _authenticationService = authenticationService;
        }

        #region -- Public properties --

        private string _login;
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _isSignInButtonEnabled;
        public bool IsSignInButtonEnabled
        {
            get => _isSignInButtonEnabled;
            set => SetProperty(ref _isSignInButtonEnabled, value);
        }


        public ICommand SignInCommand => new Command(OnSignInCommand);
        public ICommand SignUpCommand => new Command(OnSignUpCommand);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(nameof(UserModel), out UserModel userModel))
            {
                Login = userModel.Login;
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(Login) ||
                args.PropertyName == nameof(Password))
            {
                if (SignInCanExecute())
                {
                    IsSignInButtonEnabled = true;
                }
                else
                {
                    IsSignInButtonEnabled = false;
                }
            }
        }

        #endregion

        #region -- Command Handlers --

        private async void OnSignInCommand(object arg)
        {
            var result = await _authenticationService.SignInAsync(Login, Password);

            if (result.IsSuccess)
            {
                await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(MainPage)}");
            }
            else
            {
                UserDialogs.Alert("Invalid Login Or Password", "OK");
                ClearFields();
            }
        }

        private async void OnSignUpCommand()
        {
            await NavigationService.NavigateAsync($"{nameof(SignUp)}");
        }

        #endregion

        #region -- Private Helpers --

        private bool SignInCanExecute()
        {
            return
                !string.IsNullOrEmpty(Login) &&
                !string.IsNullOrEmpty(Password);
        }

        private void ClearFields()
        {
            Login = string.Empty;
            Password = string.Empty;
        }

        #endregion
    }
}

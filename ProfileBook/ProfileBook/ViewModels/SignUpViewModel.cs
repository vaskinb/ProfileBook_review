using System;
using System.ComponentModel;
using System.Windows.Input;
using Acr.UserDialogs;
using Prism.Navigation;
using ProfileBook.Models;
//using ProfileBook.Resources.Strings;
using ProfileBook.Services.Authentication;
using ProfileBook.Services.Authorization;
using ProfileBook.Validators;
using Xamarin.Forms;

namespace ProfileBook.ViewModels
{
    public class SignUpViewModel : BaseViewModel
    {
        #region -- Services --

        private readonly IAuthenticationService _authenticationService;

        #endregion

        public SignUpViewModel(INavigationService navigationService,
            IUserDialogs userDialogs,
            IAuthenticationService authenticationService
            )
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

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        private bool _isSignUpButtonEnabled;
        public bool IsSignUpButtonEnabled
        {
            get => _isSignUpButtonEnabled;
            set => SetProperty(ref _isSignUpButtonEnabled, value);
        }

        public ICommand SignUpCommand => new Command(OnSignUpCommand);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(Login) ||
                args.PropertyName == nameof(Password) ||
                args.PropertyName == nameof(ConfirmPassword))
            {
                if (SignUpCanExecute())
                {
                    IsSignUpButtonEnabled = true;
                }
                else
                {
                    IsSignUpButtonEnabled = false;
                }
            }
        }

        #endregion

        #region -- Command Handlers --

        private async void OnSignUpCommand(object obj)
        {
            if (Password == ConfirmPassword)
            {
                string alertMessage = null;

                var loginValid = Validator.ValidateStringAs(Login, Validator.Login);
                var passwordValid = Validator.ValidateStringAs(Password, Validator.Password);

                if (!loginValid)
                {
                    alertMessage = "Login Validation Error";
                }
                else if (!passwordValid)
                {
                    alertMessage = "Password Validation Error";
                    ClearPasswordFields();
                }


                if (loginValid && passwordValid)
                {
                    var authResult = await _authenticationService.SignUpAsync(Login, Password);

                    if (authResult.IsSuccess)
                    {
                        var parameters = new NavigationParameters();
                        parameters.Add(nameof(UserModel), authResult.Result);

                        await NavigationService.GoBackAsync(parameters);
                    }
                    else
                    {
                        UserDialogs.Alert("Login is already use");
                        Login = string.Empty;
                    }
                }
                else
                {
                    UserDialogs.Alert(alertMessage);
                }
            }
            else
            {
                UserDialogs.Alert("Confirm password Mismatch Error", "OK");
                ClearPasswordFields();
            }
        }

        #endregion


        #region -- Private Helpers --

        private bool SignUpCanExecute()
        {
            return
                !string.IsNullOrEmpty(Login) &&
                !string.IsNullOrEmpty(Password) &&
                !string.IsNullOrEmpty(ConfirmPassword);
        }

        private void ClearPasswordFields()
        {
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }

        #endregion
    }
}

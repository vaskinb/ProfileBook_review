using System.Threading.Tasks;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using ProfileBook.Views;
using ProfileBook.ViewModels;
using Xamarin.Forms;
using Acr.UserDialogs;
using ProfileBook.Services.Repository;
using ProfileBook.Services.Settings;
using Plugin.Settings.Abstractions;
using Plugin.Settings;
using ProfileBook.Services.Authorization;
using ProfileBook.Services.Authentication;
using ProfileBook.Services.Profile;

namespace ProfileBook
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null)
           : base(initializer)
        {
        }

        #region -- Services --

        private IAuthorizationService _authorizationService;
        private IAuthorizationService AuthorizationService =>
            _authorizationService ??= Container.Resolve<IAuthorizationService>();

        #endregion

        #region -- Ovverides --

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<SignIn, SignInViewModel>();
            containerRegistry.RegisterForNavigation<SignUp, SignUpViewModel>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<AddEditProfile, AddEditProfileViewModel>();

            //Services
            containerRegistry.RegisterInstance<ISettingsManager>(Container.Resolve<SettingsManager>());
            containerRegistry.RegisterInstance<IRepository>(Container.Resolve<Repository>());
            containerRegistry.RegisterInstance<IAuthorizationService>(Container.Resolve<AuthorizationService>());
            containerRegistry.RegisterInstance<IAuthenticationService>(Container.Resolve<AuthenticationService>());
            containerRegistry.RegisterInstance<IProfileService>(Container.Resolve<ProfileService>());

            //Packages
            containerRegistry.RegisterInstance<ISettings>(CrossSettings.Current);
            containerRegistry.RegisterInstance<IUserDialogs>(UserDialogs.Instance);


        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await InitNavigationAsync();
        }

        #endregion

        #region -- Private Helpers -- 

        private async Task InitNavigationAsync()
        {
            if (AuthorizationService.IsAuthorized)
            {
                await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(MainPage)}");
            }
            else
            {
                await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(SignIn)}");
            }
        }

        #endregion
    }
}

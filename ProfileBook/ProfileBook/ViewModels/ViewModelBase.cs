using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Prism.Mvvm;
using Prism.Navigation;


namespace ProfileBook.ViewModels
{
    public class BaseViewModel :BindableBase, INavigatedAware, IDestructible, IInitializeAsync, IInitialize
    {
        #region -- Services --

        protected INavigationService NavigationService { get; }
        protected IUserDialogs UserDialogs { get; }

        #endregion

        public BaseViewModel( INavigationService navigationService,
                              IUserDialogs userDialogs)
        {
            NavigationService = navigationService;
            UserDialogs = userDialogs;
        }

        #region --- implementations ---

        public virtual async void Initialize(INavigationParameters parameters)
        {

        }
        public virtual async Task InitializeAsync(INavigationParameters parameters)
        {

        }
        public virtual void Destroy()
        {

        }
        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }
        public virtual async void OnNavigatedTo(INavigationParameters parameters)
        {

        }
        public virtual void OnAppearing()
        {

        }
        public virtual void OnDisappearing()
        {

        }
        #endregion
    }
}
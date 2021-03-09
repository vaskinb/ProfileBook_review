using Xamarin.Forms;

namespace ProfileBook.Views
{
    public partial class SignIn : ContentPage
    {
        public SignIn()
        {
            InitializeComponent();
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
           // signInButton.IsEnabled = loginInput.Text != null && passwordInput.Text != null;
        }
    }
}

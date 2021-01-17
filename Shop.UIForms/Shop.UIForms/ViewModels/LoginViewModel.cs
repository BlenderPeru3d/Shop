
namespace Shop.UIForms.ViewModels
{
    using System;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Shop.UIForms.Views;
    using Xamarin.Forms;

    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public ICommand LoginCommand => new RelayCommand(Login);

        public LoginViewModel()
        {
            Email = "peru3dblender@gmail.com";
            Password = "123456";
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter an Email",
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter a Password",
                    "Accept");
                return;
            }

            if (!Email.Equals("peru3dblender@gmail.com")|| !Password.Equals("123456"))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"user or password wrong{Email} {Password}",
                    "Accept");
                return;
            }
            //await Application.Current.MainPage.DisplayAlert(
            //        "OK",
            //        "Taraaaannnnnn!!!!1",
            //        "Accept");
            await Application.Current.MainPage.Navigation.PushAsync(new ProductsPage())
        }
    }
}


namespace Shop.UIForms.ViewModels
{
    using Shop.Common.Models;
    using Shop.Common.Services;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Xamarin.Forms;

    public class ProductsViewModel: BaseViewModel
    {
        private readonly ApiService apiService;
        private ObservableCollection<Product> products;
        public ObservableCollection<Product> Products
        {
            get => products;
            set => SetValue(ref products, value);
        }

        public ProductsViewModel()
        {
            apiService = new ApiService();
            LoadProductsAsync();
        }

        private async void LoadProductsAsync()
        {
            Response response = await apiService.GetListAsync<Product>(
                "https://shopwebtutorial.azurewebsites.net",
                "/api",
                "/Products");
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                return;
            }
            List<Product> myProducts = (List<Product>)response.Result;
            Products = new ObservableCollection<Product>(myProducts);
        }
    }
}

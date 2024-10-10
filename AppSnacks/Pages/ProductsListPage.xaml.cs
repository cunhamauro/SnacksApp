using AppSnacks.Models;
using AppSnacks.Services;
using AppSnacks.Validations;
using System.Runtime.CompilerServices;

namespace AppSnacks.Pages;

public partial class ProductsListPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _categoryId;
    private bool _loginPageDisplayed = false;

    public ProductsListPage(int categoryId, string categoryName, ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;

        _categoryId = categoryId;
        Title = categoryName ?? "Products";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductsList(_categoryId);
    }

    private async Task<IEnumerable<Product>> GetProductsList(int categoryId)
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("category", categoryId.ToString());

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            CvProducts.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There was an unexpected error: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;

        if (currentSelection == null) { return; };

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.Id, currentSelection.Name!, _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}
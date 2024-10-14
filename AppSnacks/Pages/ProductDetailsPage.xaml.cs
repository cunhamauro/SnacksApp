using AppSnacks.Models;
using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class ProductDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly int _productId;
    private bool _loginPageDisplayed = false;

    public ProductDetailsPage(int productId, string productName, ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;

        _productId = productId;
        Title = productName ?? "Products";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductDetails(_productId);
    }

    private async Task<Product?> GetProductDetails(int productId)
    {
        var (productDetails, errorMessage) = await _apiService.GetProductDetails(productId);

        if(errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        if (productDetails is null)
        {
            await DisplayAlert("Error", errorMessage ?? "The product could not be fetched!", "OK");
            return null;
        }

        if (productDetails != null)
        {
            ProductImage.Source = productDetails.ImagePath;
            LblProductName.Text = productDetails.Name;
            LblProductPrice.Text = productDetails.Price.ToString();
            LblProductDescription.Text = productDetails.Details;
            LblTotalPrice.Text = productDetails.Price.ToString();   
        }
        else
        {
            await DisplayAlert("Error", errorMessage ?? "The product could not be fetched!", "OK");
            return null;
        }
        return productDetails;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void BtnFavourite_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) && decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            quantity = Math.Max(1, quantity -1);
            LblQuantity.Text = quantity.ToString();

            var totalPrice = quantity * unitPrice;
            LblTotalPrice.Text = totalPrice.ToString();
        }
        else
        {
            DisplayAlert("Error", "Invalid values", "OK");
        }
    }

    private void BtnAdd_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) && decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            quantity++;
            LblQuantity.Text = quantity.ToString();

            var totalPrice = quantity * unitPrice;
            LblTotalPrice.Text = totalPrice.ToString();
        }
        else
        {
            DisplayAlert("Error", "Invalid values", "OK");
        }
    }

    private async void BtnAddToCart_Clicked(object sender, EventArgs e)
    {
        try
        {
            var cart = new ShoppingCart()
            {
                Quantity = Convert.ToInt32(LblQuantity.Text),
                UnitPrice = Convert.ToDecimal(LblProductPrice.Text),
                TotalPrice = Convert.ToDecimal(LblTotalPrice.Text),
                ProductId = _productId,
                ClientId = Preferences.Get("userid", 0),
            };

            var response = await _apiService.AddItemToCart(cart);

            if (response.Data)
            {
                await DisplayAlert("Success", "Item added to cart!", "OK");

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", $"Failure adding item to cart: {response.ErrorMessage}", "OK");
            }
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error", $"There was an unexpected error: {ex.Message}", "OK");
        }
    }
}
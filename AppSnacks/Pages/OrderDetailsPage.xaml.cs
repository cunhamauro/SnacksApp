using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class OrderDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoritesService _favoritesService;
    private bool _loginPageDisplayed = false;

    public OrderDetailsPage(int orderId, decimal totalOrder, ApiService apiService, IValidator validator, FavoritesService favoritesService)
    {
        InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
        _favoritesService = favoritesService;
        LblTotalPrice.Text = "$" + totalOrder.ToString();

        GetOrderDetail(orderId);
    }

    private async void GetOrderDetail (int orderId)
    {
        try
        {
            loadIndicator.IsRunning = true;
            loadIndicator.IsVisible = true;

            var (orderDetail, errorMessage) = await _apiService.GetOrderDetail(orderId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (orderDetail is null)
            {
                await DisplayAlert("Error", errorMessage ?? "The order detail could not be fetched", "OK");
                return;
            }
            else
            {
                CvOrderDetails.ItemsSource = orderDetail;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error fetching order detail. Try again later", "OK");
        }
        finally
        {
            loadIndicator.IsRunning = false;
            loadIndicator.IsVisible = false;
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator,_favoritesService));
    }
}
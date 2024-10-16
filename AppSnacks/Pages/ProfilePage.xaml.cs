using AppSnacks.Models;
using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoritesService _favoritesService;
    private bool _loginPageDisplayed = false;

    public ProfilePage(ApiService apiService, IValidator validator, FavoritesService favoritesService)
	{
        InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
        _favoritesService = favoritesService;
        LblUserName.Text = Preferences.Get("username", string.Empty);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ImgBtnProfile.Source = await GetProfileImage();
    }

    private async Task<string?> GetProfileImage()
    {
        string standardImage = AppConfig.DefaultProfileImage;

        var (response, errorMessage) = await _apiService.GetImageProfileUser();

        if (response is null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    }
                    break;
                default:
                    await DisplayAlert("Error", errorMessage ?? "The profile image could not be fetched!", "OK");
                    return standardImage;
            }
        }

        if (response?.ImageUrl is not null)
        {
            return response.ImagePath;
        }

        return standardImage;
    }

    private async void ImgBtnProfile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imageArray = await SelectImageAsync();
            if (imageArray == null)
            {
                await DisplayAlert("Error", "Error loading the image", "OK");
                return;
            }
            ImgBtnProfile.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

            var response = await _apiService.UploadUserImage(imageArray);

            if (response.Data)
            {
                await DisplayAlert("", "Image successfully uploaded", "OK");
            }
            else
            {
                await DisplayAlert("Error", response.ErrorMessage ?? "There was an unexpected error", "Cancel");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error",  $"There was an unexpected error: {ex.Message}", "OK");
        }
    }

    private async Task<byte[]> SelectImageAsync()
    {
        try
        {
            var archive = await MediaPicker.PickPhotoAsync();

            if (archive is null)
            {
                return null;
            }

            using(var stream = await archive.OpenReadAsync())
            using(var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Error", $"The functionality is not supported in this device", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Error", $"Permission not granted to access camera or gallery", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There was an error selecting the image: {ex.Message}", "OK");
        }
        return null;
    }

    private void TapOrders_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new OrdersPage(_apiService, _validator, _favoritesService));
    }

    private void MyAccount_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new MyAccountPage(_apiService));
    }

    private void Questions_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new FaqPage());
    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("accesstoken", string.Empty);
        Application.Current!.MainPage = new NavigationPage(new LoginPage(_apiService, _validator, _favoritesService));
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favoritesService));
    }
}
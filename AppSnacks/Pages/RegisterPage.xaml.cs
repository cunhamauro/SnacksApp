using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoritesService _favoritesService;

    public RegisterPage(ApiService apiService, IValidator validator, FavoritesService favoritesService)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favoritesService = favoritesService;
    }

    private async void BtnSignup_Clicked(object sender, EventArgs e)
    {
        bool valid = await _validator.Validate(EntName.Text, EntEmail.Text, EntPhone.Text, EntPassword.Text);

        if (valid)
        {
            var response = await _apiService.RegisterUser(EntName.Text, EntEmail.Text, EntPhone.Text, EntPassword.Text);

            if (!response.HasError)
            {
                await DisplayAlert("Info", "Your account was created successfully!", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favoritesService));
            }
            else
            {
                await DisplayAlert("Error", "Something went wrong!", "Cancel");
            }
        }
        else
        {
            string errorMessage = "";

            errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
            errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            errorMessage += _validator.PhoneNumberError != null ? $"\n- {_validator.PhoneNumberError}" : "";
            errorMessage += _validator.PasswordError != null ? $"\n- {_validator.PasswordError}" : "";

            await DisplayAlert("Error", errorMessage, "OK");
        }
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favoritesService));
    }
}
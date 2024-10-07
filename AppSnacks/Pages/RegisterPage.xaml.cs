using AppSnacks.Services;

namespace AppSnacks.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;

    public RegisterPage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }

    private async void BtnSignup_Clicked(object sender, EventArgs e)
    {
        var response = await _apiService.RegisterUser(EntName.Text, EntEmail.Text,
                                                  EntPhone.Text, EntPassword.Text);

        if (!response.HasError)
        {
            await DisplayAlert("Info", "Your account was created successfully!", "OK");
            await Navigation.PushAsync(new LoginPage(_apiService));
        }
        else
        {
            await DisplayAlert("Error", "Something went wrong!", "Cancel");
        }
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService));

    }
}
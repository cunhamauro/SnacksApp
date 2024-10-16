using AppSnacks.Models;
using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class MyAccountPage : ContentPage
{
    private readonly ApiService _apiService;

    private const string NameUserKey = "username";
    private const string EmailUserKey = "email";
    private const string PhoneUserKey = "phonenumber";

    public MyAccountPage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoadUserInfo();
        ImgBtnProfile.Source = await GetProfileImageAsync();
    }

    private async Task<string?> GetProfileImageAsync()
    {
        string standardImage = AppConfig.DefaultProfileImage;

        var (response, errorMessage) = await _apiService.GetImageProfileUser();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Error", "Unauthorized", "OK");
                    return standardImage;
                default:
                    await DisplayAlert("Error", errorMessage ?? "Could not fetch profile image", "OK");
                    return standardImage;
            }
        }

        if (response?.ImageUrl is not null)
        {
            return response.ImagePath;
        }
        return standardImage;
    }


    private void LoadUserInfo()
    {
        LblUserName.Text = Preferences.Get(NameUserKey, string.Empty);
        EntName.Text = LblUserName.Text;
        EntEmail.Text = Preferences.Get(EmailUserKey, string.Empty);
        EntPhone.Text = Preferences.Get(PhoneUserKey, string.Empty);
    }


    private async void Button_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(NameUserKey, EntName.Text);
        Preferences.Set(EmailUserKey, EntEmail.Text);
        Preferences.Set(PhoneUserKey, EntPhone.Text);
        await DisplayAlert("Data saved", "Your informations were successfully saved", "OK");
    }
}
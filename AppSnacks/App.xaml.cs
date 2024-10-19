using AppSnacks.Pages;
using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private readonly FavoritesService _favoritesService;

        public App(ApiService apiService, IValidator validator, FavoritesService favoritesService)
        {
            InitializeComponent();

            // Force light theme
            UserAppTheme = AppTheme.Light;

            _apiService = apiService;
            _validator = validator;
            _favoritesService = favoritesService;
            SetMainPage();
        }

        private void SetMainPage()
        {
            var accessToken = Preferences.Get("accesstoken", string.Empty);

            if (string.IsNullOrEmpty(accessToken))
            {
                MainPage = new NavigationPage(new RegisterPage(_apiService, _validator, _favoritesService));
                return;
            }

            MainPage = new AppShell(_apiService, _validator, _favoritesService);
        }
    }
}

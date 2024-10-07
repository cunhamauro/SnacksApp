using AppSnacks.Pages;
using AppSnacks.Services;

namespace AppSnacks
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;

        public App(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;

            MainPage = new NavigationPage(new RegisterPage(_apiService));
        }
    }
}

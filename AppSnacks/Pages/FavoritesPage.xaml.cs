using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class FavoritesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public FavoritesPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
    }
}
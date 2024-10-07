using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class CartPage : ContentPage
{
    private readonly IValidator _validator;
    private readonly ApiService _apiService;

    public CartPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _validator = validator;
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
    }
}
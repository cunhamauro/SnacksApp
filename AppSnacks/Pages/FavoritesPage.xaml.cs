using AppSnacks.Models;
using AppSnacks.Services;
using AppSnacks.Validations;

namespace AppSnacks.Pages;

public partial class FavoritesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoritesService _favoritesService;

    public FavoritesPage(ApiService apiService, IValidator validator, FavoritesService favoritesService)
    {
        InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
        _favoritesService = favoritesService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductsFavorite();
    }

    private async Task GetProductsFavorite()
    {
        try
        {
            var productsFavorite = await _favoritesService.ReadAllAsync();

            if (productsFavorite is null || productsFavorite.Count == 0)
            {
                CvProducts.ItemsSource = null;
                LblWarning.IsVisible = true;
            }
            else
            {
                CvProducts.ItemsSource = productsFavorite;
                LblWarning.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There was an unexpected error: {ex.Message}", "OK");
        }

    }

    private void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as ProductFavorite;

        if (currentSelection == null) return;

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.ProductId,
                                                     currentSelection.Name!,
                                                     _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}
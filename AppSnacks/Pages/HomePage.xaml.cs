using AppSnacks.Models;
using AppSnacks.Services;
using AppSnacks.Validations;
using System.Net;
using System.Text.Json;
using System.Text;

namespace AppSnacks.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoritesService _favoritesService;
    private bool _isDataLoaded = false;

    private bool _loginPageDisplayed = false;

    public HomePage(ApiService apiService, IValidator validator, FavoritesService favoritesService)
    {
        InitializeComponent();
        LblUserName.Text = "Hello," + Preferences.Get("username", string.Empty);

        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
        _favoritesService = favoritesService;
        Title = AppConfig.HomePageTitle;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_isDataLoaded)
        {
            await LoadDataAsync();
            _isDataLoaded = true;
        }
    }

    private async Task LoadDataAsync()
    {
        var categoriesTask = GetListCategories();
        var mostSoldTask = GetMostSold();
        var popularTask = GetPopular();

        await Task.WhenAll(categoriesTask, mostSoldTask, popularTask);
    }

    private async Task<IEnumerable<Product>> GetPopular()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("popular", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not fetch the products", "OK");
                return Enumerable.Empty<Product>();
            }

            CvPopular.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task<IEnumerable<Product>> GetMostSold()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("mostsold", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not fetch the products", "OK");
                return Enumerable.Empty<Product>();
            }

            CvMostSold.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task<IEnumerable<Category>> GetListCategories()
    {
        try
        {
            var (categories, errorMessage) = await _apiService.GetCategories();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Category>();
            }

            if (categories == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not fetch the categories", "OK");
                return Enumerable.Empty<Category>();
            }

            CvCategories.ItemsSource = categories;
            return categories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
            return Enumerable.Empty<Category>();
        }
    }


    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favoritesService));
    }


    private void CvCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Category;

        if (currentSelection is null) { return; };

        Navigation.PushAsync(new ProductsListPage(currentSelection.Id,
            currentSelection.Name!,
            _apiService, _validator, _favoritesService));

        ((CollectionView)sender).SelectedItem = null;
    }

    private void CvMostSold_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            NavigateToProductDetailsPage(collectionView, e);
        }
    }

    private void CvPopular_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            NavigateToProductDetailsPage(collectionView, e);
        }
    }

    private void NavigateToProductDetailsPage(CollectionView collectionView, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;

        if (currentSelection is null)
        {
            return;
        }

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.Id, currentSelection.Name!, _apiService, _validator));

        collectionView.SelectedItem = null;
    }
}
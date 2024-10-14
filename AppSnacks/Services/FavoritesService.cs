using AppSnacks.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSnacks.Services
{
    public class FavoritesService
    {
        private readonly SQLiteAsyncConnection _database;

        public FavoritesService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "favorites.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<ProductFavorite>().Wait();
        }

        public async Task<ProductFavorite> ReadAsync(int id)
        {
            try
            {
                return await _database.Table<ProductFavorite>().Where(p => p.ProductId == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ProductFavorite>> ReadAllAsync()
        {
            try
            {
                return await _database.Table<ProductFavorite>().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateAsync(ProductFavorite productFavorite)
        {
            try
            {
                await _database.InsertAsync(productFavorite);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(ProductFavorite productFavorite)
        {
            try
            {
                await _database.DeleteAsync(productFavorite);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

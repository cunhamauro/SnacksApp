using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AppSnacks.Models
{
    public class ShoppingCartItem : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get; set; }

        private int _quantity;

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? ImageUrl { get; set; }

        public string? ImagePath => AppConfig.Url + ImageUrl;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
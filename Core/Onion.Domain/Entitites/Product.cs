using Onion.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Domain.Entitites
{
    public class Product: EntityBase
    {
        public Product()
        {
            
        }

        public Product(string title, string description, decimal price, int brandId, decimal discount)
        {
            Title = title;
            Description = description;
            Price = price;
            BrandId = brandId;
            Discount = discount;
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public decimal Discount { get; set; }

        public Brand Brand { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
        //public required string ImagePath { get; set; }
    }
}

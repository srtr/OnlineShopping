using System.Linq;
using OnlineShopping.Domain.Entities;

namespace OnlineShopping.Domain.Abstract
{
    public interface IProductRepository
    {
        IQueryable<Product> Products { get; }
        void saveProduct(Product product);
        void quickSaveProduct(Product product);
        void deleteProduct(Product product);
        void saveContext();
        void deleteTable();
    }
}
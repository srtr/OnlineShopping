using System.Linq;
using OnlineShopping.Domain.Abstract;
using OnlineShopping.Domain.Entities;
using System.Data.Entity;

namespace OnlineShopping.Domain.Concrete
{
    public class EFProductRepository : IProductRepository
    {
        private EFDbContext context = new EFDbContext();
        public IQueryable<Product> Products
        {
            get { return context.Products; }
        }

        public void quickSaveProduct(Product product)
        {
            if (product.productID == 0)
            {
                context.Products.Add(product);
            }
            else
            {
                context.Entry(product).State = EntityState.Modified;

            }
        }

        public void saveProduct(Product product)
        {
            if (product.productID == 0)
            {
                context.Products.Add(product);
                context.SaveChanges();
            }
            else
            {
                context.Entry(product).State = EntityState.Modified;
                context.SaveChanges();


            }
        }

        public void saveContext()
        {
            context.SaveChanges();
        }

        public void deleteProduct(Product product)
        {
            context.Products.Remove(product);
            context.SaveChanges();
        }

        public void deleteTable()
        {
            context.Database.ExecuteSqlCommand("TRUNCATE TABLE PRODUCTS");

        }

    }
}
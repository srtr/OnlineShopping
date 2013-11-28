using OnlineShopping.Domain.Abstract;
using OnlineShopping.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Domain.Concrete
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private EFDbContext context = new EFDbContext();
        public IQueryable<Category> Categories
        {
            get { return context.Categories; }
        }

        public void quickSaveCategory(Category category)
        {
            if (category.categoryID == 0)
            {
                context.Categories.Add(category);
                // context.SaveChanges();
            }
            else
            {
                context.Entry(category).State = EntityState.Modified;
                // context.SaveChanges();
            }
        }

        public void saveCategory(Category category)
        {
            if (category.categoryID == 0)
            {
                context.Categories.Add(category);
                context.SaveChanges();
            }
            else
            {
                context.Entry(category).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void saveContext()
        {
            context.SaveChanges();
        }

        public void deleteCategory(Category category)
        {
            context.Categories.Remove(category);
            context.SaveChanges();
        }

        public void deleteTable()
        {
            context.Database.ExecuteSqlCommand("TRUNCATE TABLE CATEGORIES");

        }
    }
}

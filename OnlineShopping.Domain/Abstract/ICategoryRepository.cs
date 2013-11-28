using OnlineShopping.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Domain.Abstract
{
    public interface ICategoryRepository
    {
        IQueryable<Category> Categories { get; }
        void saveCategory(Category category);
        void quickSaveCategory(Category category);
        void deleteCategory(Category category);
        void saveContext();
        void deleteTable();
    }
}

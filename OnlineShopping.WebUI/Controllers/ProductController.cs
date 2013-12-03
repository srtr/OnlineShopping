using System.Linq;
using System.Web.Mvc;
using OnlineShopping.Domain.Abstract;
using OnlineShopping.WebUI.Models;
using System.Collections.Generic;

namespace OnlineShopping.WebUI.Controllers
{
    public class ProductController : Controller
    {
        public int PageSize = 10;
        private IProductRepository _productRepo;
        private ICategoryRepository _categoryRepo;

        public ProductController(IProductRepository productRepository,ICategoryRepository categoryRepository)
        {
            _productRepo = productRepository;
            _categoryRepo = categoryRepository;
        }
        public ViewResult List(string categoryName,int page=1)
        {
            int tmpCategoryID;
            if (categoryName == null)
                tmpCategoryID = 0;
            else
                tmpCategoryID = _categoryRepo.Categories.Where(p => p.categoryName == categoryName).Select(p => p.categoryID).First();

            ProductsListViewModel viewModel = new ProductsListViewModel
            {
                CurrentCategoryName = categoryName,
                CurrentCategoryID = tmpCategoryID,

                Products = _productRepo.Products
                    .Where(p => tmpCategoryID <= 0 || p.categoryID == tmpCategoryID)
                    .OrderBy(p => p.productID)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
              
                PagingInfo = new PagingInfo { 
                    CurrentPage = page, 
                    ItemsPerPage = PageSize,
                    TotalItems = categoryName == null ? _productRepo.Products.Count() : _productRepo.Products.Where(e => e.categoryID == tmpCategoryID).Count() 
                }                
            };
            return View(viewModel);
        }

    }
}
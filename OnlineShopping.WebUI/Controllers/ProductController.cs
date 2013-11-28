using System.Linq;
using System.Web.Mvc;
using OnlineShopping.Domain.Abstract;
using OnlineShopping.WebUI.Models;

namespace OnlineShopping.WebUI.Controllers
{
    public class ProductController : Controller
    {
        public int PageSize = 4;
        private IProductRepository _productRepo;
        public ProductController(IProductRepository productRepository)
        {
            _productRepo = productRepository;
        }
        public ViewResult List(string category,int page=1)
        {
            ProductsListViewModel viewModel = new ProductsListViewModel
            {
                Products = _productRepo.Products
                   // .Where(p => category == null || p.categoryName == category)
                    .OrderBy(p => p.productID)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
              
                PagingInfo = new PagingInfo { 
                    CurrentPage = page, 
                    ItemsPerPage = PageSize, 
                    TotalItems = _productRepo.Products.Count() 
                },

                CurrentCategory = category
            };
            return View(viewModel);
        }

    }
}
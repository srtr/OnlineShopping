using System.Linq;
using System.Web.Mvc;
using OnlineShopping.Domain.Abstract;
using OnlineShopping.WebUI.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System;

namespace OnlineShopping.WebUI.Controllers
{
    public class ProductController : Controller
    {
        public int PageSize = 10;
        private IProductRepository _productRepo;
        private ICategoryRepository _categoryRepo;
        private IManufacturerRepository _manufacturerRepo;

        public ProductController(IProductRepository productRepository,ICategoryRepository categoryRepository, IManufacturerRepository manufacturerRepository)
        {
            _productRepo = productRepository;
            _categoryRepo = categoryRepository;
            _manufacturerRepo = manufacturerRepository;
        }
        public ViewResult Details(string productName="")
        {
            ProductsDetailsViewModel viewModel = new ProductsDetailsViewModel();
            viewModel.product = _productRepo.Products.FirstOrDefault(p => p.productName == productName);
            //viewModel.manufacturer = _manufacturerRepo.Manufacturers.FirstOrDefault(m => m.manufacturerID == viewModel.product.manufacturerID);
            //viewModel.category = _categoryRepo.Categories.FirstOrDefault(c => c.categoryID == viewModel.product.categoryID);

            return View(viewModel);
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
                    .Where(p => (tmpCategoryID <= 0 || p.categoryID == tmpCategoryID) && p.currentStock > p.minimumStock)
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

        public ContentResult getFullInventoryList()
        {

            var result = _productRepo.Products.Select(p => p.productName);

            //result.Add(i.ToString(), _productRepo.Products);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            return new ContentResult()
            {
                Content = serializer.Serialize(result), // for formatted json JsonConvert.SerializeObject(result, Formatting.Indented)
                ContentType = "application/json",
            };
        }

        public ContentResult getFullCategoriesList()
        {

            var result = _categoryRepo.Categories.Select(p => p.categoryName);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            return new ContentResult()
            {
                Content = serializer.Serialize(result),
                ContentType = "application/json",
            };
        }

        public ContentResult getFullManufacturersList()
        {

            var result = _manufacturerRepo.Manufacturers;//.Select(p => p.manufacturerName);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
            return new ContentResult()
            {
                Content = serializer.Serialize(result),
                ContentType = "application/json",
            };
        }
    }
}
using OnlineShopping.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopping.WebUI.Controllers
{
    public class NavController : Controller
    {
        private ICategoryRepository _categoryRepo; 

        public NavController(ICategoryRepository repo) { 
        _categoryRepo = repo; 
        }
        public PartialViewResult Menu(string categoryName = null)
        {
            ViewBag.SelectedCategoryName = categoryName;
            IEnumerable<string> categories = _categoryRepo.Categories 
            .Select(x => x.categoryName) 
            .Distinct() 
            .OrderBy(x => x); 
            return PartialView(categories); 
        } 
        

    }
}

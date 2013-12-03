using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShopping.Domain.Entities;
using OnlineShopping.Domain.Abstract;
using OnlineShopping.WebUI.Models;
using System.Web.Security;

namespace OnlineShopping.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IProductRepository repository;
        private IOrderProcessor orderProcessor;

        ITransactionRepository _transactionRepo;
        ITransactionDetailRepository _transactionDetailRepo;
        IProductRepository _productRepo;

        public CartController(IProductRepository repo, IOrderProcessor proc, ITransactionRepository transactionRepo, ITransactionDetailRepository transactionDetailRepo, IProductRepository productRepo)
        {
            repository = repo;
            orderProcessor = proc;
            _transactionRepo = transactionRepo;
            _transactionDetailRepo = transactionDetailRepo;
            _productRepo = productRepo;
        }

        public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl)
        {
            Product product = repository.Products
            .FirstOrDefault(p => p.productID == productId);
            if (product != null)
            {
                cart.AddItem(product, 1);
            }
            return RedirectToAction("Index", new { returnUrl });
        }
        public RedirectToRouteResult RemoveFromCart(Cart cart,
        int productId, string returnUrl)
        {
            Product product = repository.Products
            .FirstOrDefault(p => p.productID == productId);
            if (product != null)
            {
                cart.RemoveLine(product);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        public ViewResult Summary(Cart cart)
        {
            return View(cart);
        }

        private void saveCartItems(Cart cart)
        {
            foreach (var line in cart.Lines)
            {
                // Save transaction details based on cashierID
            }

        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            if (ModelState.IsValid)
            {       
                saveCartItems(cart); //To update local shop DBs with the transactions
                orderProcessor.ProcessOrder(cart, shippingDetails); //E-mail the customer regarding items purchased
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(shippingDetails);
            }
        }

        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }

    }
}

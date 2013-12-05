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

        IOnlineTransactionRepository _transactionRepo;
        IOnlineTransactionDetailRepository _transactionDetailRepo;
        IProductRepository _productRepo;

        public CartController(IProductRepository repo, IOrderProcessor proc, IOnlineTransactionRepository transactionRepo, IOnlineTransactionDetailRepository transactionDetailRepo, IProductRepository productRepo)
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

        public ViewResult Orders()
        {
            return View();
        }

        public void saveCartItems(Cart cart)
        {
            int nearestShopID;
            OnlineTransaction transaction = new OnlineTransaction();
            transaction.cashierID = 1;
            transaction.date = DateTime.Now;
            if (_transactionRepo.Transactions.Count() != 0)
                transaction.transactionID = _transactionRepo.Transactions.Max(t => t.transactionID) + 1;
            else
                transaction.transactionID = 1;

            int transactionID = getTransactionID(transaction);

            foreach (var line in cart.Lines)
            {
                //For each product find nearest store
                nearestShopID = 1;  //nearestShopID = findNearestShopID(line.Product.barcode,line.Quantity)

                // Save transaction details based on cashierID

                //Initial display of Products: Warehouse stock > minimumStock
                
                //Checkout:
                //Find nearest shop with productStock > minimumStock
                //Return shopID
                //If no shop found, order redirected to warehouse

                //nearestShopID = -1 implies warehouse. redirect stock from warehouse to nearest store to customer
                if (nearestShopID > 0)
                {
                    //connect to onlineShop DB in HQServer
                    AddTransaction(transaction, line); //saveTransactionDetails
                    //then reduce quantity from local shop DB
                }
                else
                {
                    //connect to warehouse table in HQ server
                    AddTransaction(transaction, line); //saveTransactionDetails

                    //Reduce quantity from warehouse table
                }
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

        private int getTransactionID(OnlineTransaction transaction)
        {
            _transactionRepo.saveTransaction(transaction);
            return transaction.transactionID;
        }

        public void AddTransaction(OnlineTransaction transaction, CartLine cartline)
        {
            OnlineTransactionDetail transactionDetail;
            transactionDetail = new OnlineTransactionDetail();
            transactionDetail.transactionID = transaction.transactionID;
            transactionDetail.barcode = cartline.Product.barcode;
            transactionDetail.unitSold = cartline.Quantity;
            
            Product p = _productRepo.Products.First(pd => pd.barcode == transactionDetail.barcode);
            transactionDetail.totalCost = p.costPrice * transactionDetail.unitSold;
            p.currentStock -= transactionDetail.unitSold;

            if (p.currentStock >= 0)
            {
                _transactionDetailRepo.quickSaveTransactionDetail(transactionDetail);
                _productRepo.saveProduct(p);

            }
            _transactionDetailRepo.saveContext();
        }

    }
}

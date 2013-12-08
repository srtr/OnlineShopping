using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using OnlineShopping.Domain.Entities;
using OnlineShopping.Domain.Abstract;
using OnlineShopping.WebUI.Models;
using System.Web.Security;

namespace OnlineShopping.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IProductRepository _productRepo;
        private ICategoryRepository _categoryRepo;
        private IManufacturerRepository _manufacturerRepo;
        private IOnlineTransactionRepository _transactionRepo;
        private IOnlineTransactionDetailRepository _transactionDetailRepo;
        

        public CartController(IProductRepository productRepo,ICategoryRepository categoryRepo,IManufacturerRepository manufacturerRepo, IOnlineTransactionRepository transactionRepo, IOnlineTransactionDetailRepository transactionDetailRepo)
        {
            _transactionRepo = transactionRepo;
            _transactionDetailRepo = transactionDetailRepo;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _manufacturerRepo = manufacturerRepo;
        }

        public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl)
        {
            Product product = _productRepo.Products
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
            Product product = _productRepo.Products
            .FirstOrDefault(p => p.productID == productId);
            if (product != null)
            {
                cart.RemoveLine(product);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public ViewResult Index(Cart cart, string returnUrl)
        {
            foreach (var item in cart.Lines)
            {
                item.categoryName = _categoryRepo.Categories.Where(p => p.categoryID == item.Product.categoryID).First().categoryName;
                item.manufacturerName = _manufacturerRepo.Manufacturers.Where(p => p.manufacturerID == item.Product.manufacturerID).First().manufacturerName;
            }
  
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        public ViewResult Summary(Cart cart)
        {
            int x = cart.Lines.ToArray().Length;
            return View(cart);
        }

        public ViewResult Orders()
        {
            string currentUserID = Membership.GetUser(User.Identity.Name, true /* userIsOnline */).ProviderUserKey.ToString();
            IEnumerable<OnlineTransaction> userTransactions;
            IEnumerable<OnlineTransactionDetail> userTransactionDetails = Enumerable.Empty<OnlineTransactionDetail>();
            IEnumerable<OnlineTransactionDetail> tmpDetails;
            IEnumerable<Product> productDetails = Enumerable.Empty<Product>();

           // userTransactions = _transactionRepo.Transactions.Where(p => p.userKey == currentUserID).OrderByDescending(p => p.date);
            userTransactions = _transactionRepo.Transactions.Where(p => p.userKey == "cg3002").OrderByDescending(p => p.date);
            foreach (var xact in userTransactions)
            {
                tmpDetails = _transactionDetailRepo.TransactionDetails.Where(p => p.transactionID == xact.transactionID);
                userTransactionDetails = userTransactionDetails.Concat(tmpDetails);
            }

            foreach(var item in userTransactionDetails){
                productDetails = productDetails.Concat(_productRepo.Products.Where(p => p.barcode == item.barcode));
            }

            OnlineTransactionsViewModel viewModel = new OnlineTransactionsViewModel
            {
                onlineTransactions = userTransactions,
                onlineTransactionDetails = userTransactionDetails,
                products = productDetails
            };

            return View(viewModel);
            //return View();
        }

        public ActionResult sendCustomerEmail(OnlineTransaction savedXact)
        {
            string currentUserID = Membership.GetUser(User.Identity.Name, true /* userIsOnline */).ProviderUserKey.ToString();
            IEnumerable<OnlineTransaction> userTransactions;
            IEnumerable<OnlineTransactionDetail> userTransactionDetails = Enumerable.Empty<OnlineTransactionDetail>();
            IEnumerable<Product> productDetails = Enumerable.Empty<Product>();

            // userTransactions = _transactionRepo.Transactions.Where(p => p.userKey == currentUserID && && p.transactionID == savedXact.transactionID );
            userTransactions = _transactionRepo.Transactions.Where(p => p.userKey == "cg3002" && p.transactionID == savedXact.transactionID );
            foreach (var xact in userTransactions)
            {
                userTransactionDetails = userTransactionDetails.Concat(_transactionDetailRepo.TransactionDetails.Where(p => p.transactionID == xact.transactionID));
            }

            foreach (var item in userTransactionDetails)
            {
                productDetails = productDetails.Concat(_productRepo.Products.Where(p => p.barcode == item.barcode));
            }

            var user = new OnlineTransactionsViewModel
            {
                onlineTransactions = userTransactions,
                onlineTransactionDetails = userTransactionDetails,
                products = productDetails
            };

            string userEmail = Membership.GetUser(User.Identity.Name,true).Email;
            string subject = "Transaction details for transaction ID:" + savedXact.transactionID + " dated :" + savedXact.date;
            new MailController().SampleEmail(user, subject, userEmail).Deliver();

            return RedirectToAction("LogOn","Account");

        }
        public OnlineTransaction saveCartItems(Cart cart,string shippingAddress)
        {          
            int nearestShopID;
            OnlineTransaction transaction = new OnlineTransaction();
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

            return transaction;

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
                OnlineTransaction xact= saveCartItems(cart,shippingDetails.shippingAddress); //To update local shop DBs with the transactions
//                OnlineTransaction xact = new OnlineTransaction();
//                xact.transactionID = 3;
                sendCustomerEmail(xact);

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

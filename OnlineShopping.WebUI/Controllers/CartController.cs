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
        private IOutletRepository _outletRepo;
        private IOutletInventoryRepository _outletInventoryRepo;


        public CartController(IProductRepository productRepo, ICategoryRepository categoryRepo, IManufacturerRepository manufacturerRepo, IOnlineTransactionRepository transactionRepo, IOnlineTransactionDetailRepository transactionDetailRepo, IOutletRepository outletRepo, IOutletInventoryRepository outletInventoryRepo)
        {
            _transactionRepo = transactionRepo;
            _transactionDetailRepo = transactionDetailRepo;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _manufacturerRepo = manufacturerRepo;
            _outletRepo = outletRepo;
            _outletInventoryRepo = outletInventoryRepo;
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

            userTransactions = _transactionRepo.Transactions.Where(p => p.userKey == currentUserID).OrderByDescending(p => p.date);
            //userTransactions = _transactionRepo.Transactions.Where(p => p.userKey == "cg3002").OrderByDescending(p => p.date);
            foreach (var xact in userTransactions)
            {
                tmpDetails = _transactionDetailRepo.TransactionDetails.Where(p => p.transactionID == xact.transactionID);
                userTransactionDetails = userTransactionDetails.Concat(tmpDetails);
            }

            foreach (var item in userTransactionDetails)
            {
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

            userTransactions = _transactionRepo.Transactions.Where(p => p.userKey == currentUserID && p.transactionID == savedXact.transactionID );
            //userTransactions = _transactionRepo.Transactions.Where(p => p.userKey == "cg3002" && p.transactionID == savedXact.transactionID);
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

            string userEmail = Membership.GetUser(User.Identity.Name, true).Email;
            string subject = "Transaction details for transaction ID:" + savedXact.transactionID + " dated :" + savedXact.date;
            new MailController().SampleEmail(user, subject, userEmail).Deliver();

            return RedirectToAction("LogOn", "Account");

        }

        public double Calc(double Lat1, double Long1, double Lat2, double Long2)
        {
            /*
                The Haversine formula according to Dr. Math.
                http://mathforum.org/library/drmath/view/51879.html
                
                dlon = lon2 - lon1
                dlat = lat2 - lat1
                a = (sin(dlat/2))^2 + cos(lat1) * cos(lat2) * (sin(dlon/2))^2
                c = 2 * atan2(sqrt(a), sqrt(1-a)) 
                d = R * c
                
                Where
                    * dlon is the change in longitude
                    * dlat is the change in latitude
                    * c is the great circle distance in Radians.
                    * R is the radius of a spherical Earth.
                    * The locations of the two points in 
                        spherical coordinates (longitude and 
                        latitude) are lon1,lat1 and lon2, lat2.
            */
            double dDistance = Double.MinValue;
            double dLat1InRad = Lat1 * (Math.PI / 180.0);
            double dLong1InRad = Long1 * (Math.PI / 180.0);
            double dLat2InRad = Lat2 * (Math.PI / 180.0);
            double dLong2InRad = Long2 * (Math.PI / 180.0);

            double dLongitude = dLong2InRad - dLong1InRad;
            double dLatitude = dLat2InRad - dLat1InRad;

            // Intermediate result a.
            double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                       Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                       Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // Intermediate result c (great circle distance in Radians).
            double c = 2.0 * Math.Asin(Math.Sqrt(a));

            // Distance.
            // const Double kEarthRadiusMiles = 3956.0;
            const Double kEarthRadiusKms = 6376.5;
            dDistance = kEarthRadiusKms * c;

            return dDistance;
        }

        public int findNearestShopID(string barcode, int quantity)
        {
            IEnumerable<OutletInventory> productSpecificInventory = _outletInventoryRepo.OutletInventories.Where(p => p.barcode == barcode);
            IEnumerable<OutletInventory> shopWithStock = Enumerable.Empty<OutletInventory>();

            foreach (var shop in productSpecificInventory)
            {
                if (shop.currentStock-quantity > shop.minimumStock)
                    shopWithStock = shopWithStock.Concat(productSpecificInventory.Where(p => p.outletID == shop.outletID));
            }
            double lat, lon;
            int nearestShopID = -1;
            double closestShopDist = 0.0;
            bool firstShopDist = true;
            foreach (var shop in shopWithStock)
            {
                var tmpOutlet = _outletRepo.Outlets.Where(p => p.outletID == shop.outletID).First();
                lat = tmpOutlet.latitude;
                lon = tmpOutlet.longitude;
                double tmpDist = Calc(lat, lon, 1.2987, 103.77195);
                if ((int)tmpDist < 2)
                {
                    if (firstShopDist) { closestShopDist = tmpDist; nearestShopID = shop.outletID; firstShopDist = false; }
                    if (tmpDist < closestShopDist)
                    {
                        nearestShopID = shop.outletID;
                        closestShopDist = tmpDist;
                    }
                }
            }
            return nearestShopID;
        }

        public OnlineTransaction saveCartItems(Cart cart, string shippingAddress)
        {
            int nearestShopID;
            OnlineTransaction transaction = new OnlineTransaction();
            transaction.date = DateTime.Now;
            if (_transactionRepo.Transactions.Count() != 0)
                transaction.transactionID = _transactionRepo.Transactions.Max(t => t.transactionID) + 1;
            else
                transaction.transactionID = 1;

            transaction.userKey = Membership.GetUser(User.Identity.Name, true /* userIsOnline */).ProviderUserKey.ToString();
            transaction.shippingAddress = shippingAddress;
            int transactionID = getTransactionID(transaction);


            foreach (var line in cart.Lines)
            {
                //For each product find nearest store
                nearestShopID = findNearestShopID(line.Product.barcode, line.Quantity);

                

                //Initial display of Products: Warehouse stock > minimumStock

                //Checkout:
                //Find nearest shop with productStock > minimumStock
                //Return shopID
                //If no shop found, order redirected to warehouse
                //Else Save transaction details based on shopID

                //nearestShopID = -1 implies warehouse. redirect stock from warehouse to nearest store to customer
                nearestShopID = -1;

                if (nearestShopID > 0)
                {
                    //connect to onlineShop DB in HQServer
                    AddTransaction(transaction, line); //saveTransactionDetails
                    
                    //then reduce quantity from local shop DB

                }
                else if(nearestShopID == -1)
                {
                    //connect to warehouse table in HQ server
                    AddTransaction(transaction, line); //saveTransactionDetails
                    
                    //Reduce quantity from warehouse table
                  //  OutletInventory updateOutlet = _outletInventoryRepo.OutletInventories.Where(p => p.outletID == nearestShopID).First();
              //      updateOutlet.currentStock -= line.Quantity;
                //    _outletInventoryRepo.quickUpdateOutletInventory(updateOutlet);
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
                OnlineTransaction xact = saveCartItems(cart, shippingDetails.shippingAddress); //To update local shop DBs with the transactions
                //                OnlineTransaction xact = new OnlineTransaction(); //tested Email
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

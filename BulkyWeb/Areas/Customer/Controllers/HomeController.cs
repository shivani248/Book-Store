using System.Diagnostics;
using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
[Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger , IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _uow.Product.GetAll(includeProperties:"Category");
            return View(productList);
        }
		public IActionResult Details(int id)
		{
            ShoppingCart cart = new() {
                Product = _uow.Product.Get(u => u.Id == id, includeProperties: "Category"),
                Count = 1,
                ProductId = id
            };
			
			return View(cart);
		}

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart){
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _uow.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId==shoppingCart.ProductId);
            shoppingCart.Id = 0;
            if (cartFromDb != null)
            {
            shoppingCart.Count += cartFromDb.Count;
                _uow.ShoppingCart.Update(cartFromDb);
            }
            else{
            _uow.ShoppingCart.Add(shoppingCart);
            }
            TempData["success"] = "Cart updated Successfully !!";
            _uow.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        
        public IActionResult AddCart(int id){
            Product Item = _uow.Product.Get(x => x.Id == id);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

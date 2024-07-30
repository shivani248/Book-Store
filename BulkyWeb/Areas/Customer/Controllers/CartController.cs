using System.Security.Claims;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
	[Area("customer")]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _uow;

		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CartController(IUnitOfWork uow)
		{
			_uow = uow;
		}
		public IActionResult Index()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM = new()
			{
				ShoppingCartlist = _uow.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new()
			};

			foreach (var cart in ShoppingCartVM.ShoppingCartlist)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(ShoppingCartVM);
		}

		public IActionResult Plus(int id)
		{
			var itemToBeIncremented = _uow.ShoppingCart.Get(x => x.Id == id);
			itemToBeIncremented.Count += 1;
			_uow.ShoppingCart.Update(itemToBeIncremented);
			_uow.Save();
			return RedirectToAction(nameof(Index));
		}
		public IActionResult Minus(int id)
		{
			var itemToBeDecremented = _uow.ShoppingCart.Get(x => x.Id == id);
			if (itemToBeDecremented.Count == 1)
			{
				_uow.ShoppingCart.Remove(itemToBeDecremented);
			}
			else
			{
				itemToBeDecremented.Count -= 1;
				_uow.ShoppingCart.Update(itemToBeDecremented);
			}
			_uow.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int id)
		{
			var itemToBeDeleted = _uow.ShoppingCart.Get(x => x.Id == id);
			_uow.ShoppingCart.Remove(itemToBeDeleted);
			_uow.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Summary()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM = new()
			{
				ShoppingCartlist = _uow.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new()
			};

			ShoppingCartVM.OrderHeader.ApplicationUser = _uow.ApplicationUser.Get(u => u.Id == userId);

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress =ShoppingCartVM.OrderHeader.ApplicationUser.StreetAdress;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
			foreach (var cart in ShoppingCartVM.ShoppingCartlist)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(ShoppingCartVM);
		}
		
		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM.ShoppingCartlist = _uow.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
		

		ApplicationUser applicationUser = _uow.ApplicationUser.Get(u => u.Id == userId);

			foreach (var cart in ShoppingCartVM.ShoppingCartlist)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			if(applicationUser.ComapnyId.GetValueOrDefault()==0){
				//it is regular customer account & we need to capture the payment
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			}
			else{
				//it is a company user 
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			}
			_uow.OrderHeader.Add(ShoppingCartVM.OrderHeader);
			_uow.Save();

			//create order detail 
			foreach(var cart in ShoppingCartVM.ShoppingCartlist){
				OrderDetails orderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count
				};
			_uow.OrderDetails.Add(orderDetail);
			_uow.Save();	
			}

			if (applicationUser.ComapnyId.GetValueOrDefault() == 0)
			{
				//it is regular customer account & we need to capture the payment
				//brain tree logic here
			}
            string url = string.Format("/Admin/BrainTree" , ShoppingCartVM.OrderHeader.OrderTotal);
            int totalAmount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal);
            TempData["totalAmount"] = totalAmount;

            return Redirect(url);
		}

		public IActionResult OrderConfirmation(int id){
			return View();
		}
		
		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 50)
			{
				return shoppingCart.Product.Price;
			}
			else if (shoppingCart.Count <= 100)
			{
				return shoppingCart.Product.Price50;
			}
			else
			{
				return shoppingCart.Product.Price100;
			}
		}


	}
}

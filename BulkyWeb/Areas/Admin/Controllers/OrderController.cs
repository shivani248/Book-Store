using System.Diagnostics;
using System.Security.Claims;
using Braintree;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
    {
      
      
        public readonly IUnitOfWork _uow;

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public readonly IWebHostEnvironment _webHostEnvironment;

        public OrderController(IUnitOfWork uow, IWebHostEnvironment webHostEnvironment)
        {
            _uow = uow;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId) {
            OrderVM  = new()
            {
                OrderHeader = _uow.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _uow.OrderDetails.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(OrderVM);
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin+ ","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _uow.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            if(!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier)){
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.TrackingNumber;
            }
            _uow.OrderHeader.Update(orderHeaderFromDb);
            _uow.Save();
            TempData["Success"] = "Order Detail Updated Successfully !";
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaderObj ;
            if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee)){
                orderHeaderObj = _uow.OrderHeader.GetAll(includeProperties:"ApplicationUser").ToList();
            }
            else{
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                orderHeaderObj = _uow.OrderHeader.GetAll(u=>u.ApplicationUserId == userId , includeProperties: "ApplicationUser");
            }
            

			switch (status)
			{
				case "inprocess":
					orderHeaderObj = orderHeaderObj.Where(u=> u.OrderStatus == SD.StatusInProcess).ToList();
					break;
				case "pending":
					orderHeaderObj = orderHeaderObj.Where(u => u.PaymentStatus == SD.PaymentStatusPending).ToList();
					break;
				case "completed":
					orderHeaderObj = orderHeaderObj.Where(u=> u.OrderStatus == SD.StatusShipped).ToList();
					break;
				case "approved":
					orderHeaderObj = orderHeaderObj.Where(u => u.OrderStatus == SD.StatusApproved).ToList(); ;
					break;
				default:
					break;

			}

			return Json(new { data = orderHeaderObj });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var productToBeDeleted = _uow.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            var oldImgPath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.
            ImgUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImgPath))
            {
                System.IO.File.Delete(oldImgPath);
            }
            _uow.Product.Remove(productToBeDeleted);
            _uow.Save();
            //TempData["success"] = " Product Deleted Successfully!";
            return Json(new { success = true, message = "Delete Successfully!" });
        }
        #endregion
    }
}

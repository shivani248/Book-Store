using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles =SD.Role_Admin)]
	public class ProductController : Controller
	{
		public readonly IUnitOfWork _uow;
		public readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork uow , IWebHostEnvironment webHostEnvironment)
		{
			_uow = uow;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
		{
			List<Product> productObj = _uow.Product.GetAll(includeProperties:"Category").ToList();
			return View(productObj);
		}

		public IActionResult UpdateInsert(int? id)
		{//ViewBag.CategoryListDropdownItem = CategoryList;
			ProductVM productVM = new()
			{
				CategoryListDropdownItem = _uow.Category.GetAll().Select(
				u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
				Product = new Product()
			};
			if (id == 0 || id == null)
			{
				return View(productVM);//create
			}
			else{
				productVM.Product = _uow.Product.Get(u=>u.Id==id);//update
				return View(productVM);
			}
		}
		
		[HttpPost]
		public IActionResult UpdateInsert(ProductVM productVM ,int? id, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if(file!=null){
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\products");
					var currentImgPath = Path.Combine(wwwRootPath, fileName);

					if(!string.IsNullOrEmpty(productVM.Product.ImgUrl)){
						//delete old one
						var oldImgPath = Path.Combine(wwwRootPath,productVM.Product.ImgUrl.TrimStart('\\'));
						if (System.IO.File.Exists(oldImgPath)) { 
							System.IO.File.Delete(oldImgPath);
						}
					}
					using (var fileStream = new FileStream(Path.Combine(productPath,fileName),FileMode.Create))
					{
						file.CopyTo(fileStream);
					}
					productVM.Product.ImgUrl = @"\images\products\" + fileName;
				}
				if(id == 0 || id == null){
					//create kro bhayiii
					_uow.Product.Add(productVM.Product);
					_uow.Save();
					TempData["success"] = " Product Created Successfully!";
				}
				else{
					//Update kro 
					_uow.Product.Update(productVM.Product);
					_uow.Save();
					TempData["success"] = " Product Updated Successfully!";
				}
				return RedirectToAction("Index");
			}
			else
			{
				productVM.CategoryListDropdownItem = _uow.Category.GetAll().Select(
				u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});
				return View(productVM);
			}

		}
		
		


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(int id) {
			List<Product> productObj = _uow.Product.GetAll(includeProperties: "Category").ToList();
			return Json(new { data = productObj });
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{

			var productToBeDeleted = _uow.Product.Get(u => u.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new {success = false , message = "Error while Deleting"});
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

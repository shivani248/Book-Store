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
	//[Authorize(Roles =SD.Role_Admin)]
	public class CompanyController : Controller
	{
		public readonly IUnitOfWork _uow;
		public readonly IWebHostEnvironment _webHostEnvironment;
		public CompanyController(IUnitOfWork uow , IWebHostEnvironment webHostEnvironment)
		{
			_uow = uow;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
		{
			List<Company> companyObj = _uow.Company.GetAll().ToList();
			return View(companyObj);
		}

		public IActionResult UpdateInsert(int? id)
		{//ViewBag.CategoryListDropdownItem = CategoryList;
			
			if (id == 0 || id == null)
			{
				return View(new Company());//create
			}
			else{
                Company companyObj = _uow.Company.Get(u=>u.Id==id);//update
				return View(companyObj);
			}
		}
		
		[HttpPost]
		public IActionResult UpdateInsert(Company companyObj)
		{
			if (ModelState.IsValid)
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if(companyObj.Id == 0 || companyObj.Id == null){
					//create kro bhayiii
					_uow.Company.Add(companyObj);
					_uow.Save();
					TempData["success"] = " Product Created Successfully!";
				}
				else{
					//Update kro  
					_uow.Company.Update(companyObj);
					_uow.Save();
					TempData["success"] = " Product Updated Successfully!";
				}
				return RedirectToAction("Index");
			}
			else
			{
				return View(companyObj);
			}

		}
		  
		


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(int id) {
			List<Company> companytObj = _uow.Company.GetAll().ToList();
			return Json(new { data = companytObj });
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{

			var productToBeDeleted = _uow.Company.Get(u => u.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new {success = false , message = "Error while Deleting"});
			}
			
			_uow.Company.Remove(productToBeDeleted);
			_uow.Save();
			//TempData["success"] = " Product Deleted Successfully!";
			return Json(new { success = true, message = "Delete Successfully!" });
		}
		#endregion
	}
}

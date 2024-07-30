using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
	//[Authorize(Roles = SD.Role_Admin)]
	public class CategoryController : Controller
    {
        private readonly IUnitOfWork _uow;

        public CategoryController(IUnitOfWork uow)
        {
            _uow = uow;

        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _uow.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            Category ctg = new Category();
            ctg = null;
            return View("Create", ctg);
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //	ModelState.AddModelError("name", "The display order can not exactly match the name.");
            //}
            if (ModelState.IsValid)
            {
                _uow.Category.Add(obj);
                _uow.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDb = _uow.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View("Create", categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            _uow.Category.Update(obj);
            _uow.Save();
            TempData["success"] = "Category Updated Successfuly!";
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryItemToBeDeleted = _uow.Category.Get(u => u.Id == id);
            if (categoryItemToBeDeleted == null)
            {
                return NotFound();
            }
            _uow.Category.Remove(categoryItemToBeDeleted);
            _uow.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }



    }


}

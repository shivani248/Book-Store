using BulkyWebRazorPage.Data;
using BulkyWebRazorPage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazorPage.Pages.Categories
{
	[BindProperties]
	public class EditModel : PageModel
	{
		private readonly ApplicationDbContext _db;

		public Category? CategoryToBeEdited { get; set; }
		public EditModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet(int? Id)
		{
			if (Id != null && Id != 0)
			{
				CategoryToBeEdited = _db.Categories.Find(Id);
			}

		}

		public IActionResult OnPost()
		{
			if (ModelState.IsValid) { 
			_db.Categories.Update(CategoryToBeEdited);
			_db.SaveChanges();
			TempData["success"] = "Category Updated Successfully !";
			return RedirectToPage("Index");
			}
			return Page();
		}
	}
}

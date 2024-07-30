using BulkyWebRazorPage.Data;
using BulkyWebRazorPage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazorPage.Pages.Categories
{
	public class DeleteModel : PageModel
	{
		private readonly ApplicationDbContext _db;

		[BindProperty]
		public Category CategoryToBeEdited { get; set; }
		public DeleteModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet(int Id)
		{
			CategoryToBeEdited = _db.Categories.Find(Id);

		}

		public IActionResult OnPost()
		{
			_db.Categories.Remove(CategoryToBeEdited);
			_db.SaveChanges();
			TempData["success"] = "Category Deleted Successfully !";
			return RedirectToPage("Index");
		}
	}
}

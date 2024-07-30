using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		public readonly ApplicationDbContext _db;
		public ProductRepository(ApplicationDbContext db) : base(db)
		{
			_db = db; 
		}

		public void Update(Product productObj)
		{
			var objFromDb = _db.Products.FirstOrDefault(u=>u.Id== productObj.Id);
			if (objFromDb != null) {
				objFromDb.Title = productObj.Title;
				objFromDb.ISBN = productObj.ISBN;
				objFromDb.Price = productObj.Price;
				objFromDb.Price50 = productObj.Price50;
				objFromDb.Price100 = productObj.Price100;
				objFromDb.Description = productObj.Description;
				objFromDb.CategoryId = productObj.CategoryId;
				objFromDb.Author = productObj.Author;
				if (productObj.ImgUrl != null) { 
				objFromDb.ImgUrl= productObj.ImgUrl;
				}
			}
			
		}
	}
}

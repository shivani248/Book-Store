﻿   using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
	public class CategoryRepository :Repository<Category>, ICategoryRepository
	{

		public readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
			_db = db; 
        }
       

		public void Update(Category categoryObj)
		{
			_db.Categories.Update(categoryObj);
		}
	}
}

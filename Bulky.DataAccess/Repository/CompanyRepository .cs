   using System;
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
	public class CompanyRepository :Repository<Company>, ICompanyRepository
    {

		public readonly ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
			_db = db; 
        }
       

		public void Update(Company categoryObj)
		{
			_db.Companies.Update(categoryObj);
		}
	}
}

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
	public class OrderHeaderRepository :Repository<OrderHeader>, IOrderHeaderRepository
    {

		public readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
			_db = db; 
        }
       

		public void Update(OrderHeader orderHeaderRepositoryObj)
		{
			_db.OrderHeaders.Update(orderHeaderRepositoryObj);
		}
	}
}

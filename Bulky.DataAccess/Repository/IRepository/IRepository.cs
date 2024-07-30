using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
	public interface IRepository<T> where T : class
	{
	//T - Category or any other model on which we want to perform the CRUD Operation or rather we want to incteract dbcontext 
	 IEnumerable<T> GetAll(  Expression<Func<T, bool>>? filter=null , string? includeProperties = null);	
	 T Get(Expression<Func<T, bool>> filter , string? includeProperties = null , bool tracked = false);
	 void Add(T entity);
	 void Remove(T entity);
	 void RemoveRanges (IEnumerable<T> entities);
	}
}

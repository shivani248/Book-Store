﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
	public class ShoppingCartVM
	{
		public IEnumerable<ShoppingCart> ShoppingCartlist { get; set; }
		public OrderHeader OrderHeader { get; set; }
		//public double OrderTotal { get; set; }
	}
}

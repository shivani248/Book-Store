using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Braintree;

namespace BulkyBook.Utility
{
	public interface IBrainTreeGate
	{
		IBraintreeGateway CreateGateway();
		IBraintreeGateway GetGateway();
	}
}

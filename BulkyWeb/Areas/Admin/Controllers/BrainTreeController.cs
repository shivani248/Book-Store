using Braintree;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
[Area("Admin")]
	public class BrainTreeController : Controller
	{
		public IBrainTreeGate _brain {  get; set; }
		public readonly IUnitOfWork _uow;

        
        public BrainTreeController(IBrainTreeGate brain , IUnitOfWork uow)
        {
            _brain = brain;
			_uow = uow;
        }
        public IActionResult Index()
		{
			var gateway = _brain.GetGateway();
			var clientToken = gateway.ClientToken.Generate();
			ViewBag.ClientToken = clientToken;
		    
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(IFormCollection collection){
            Random rnd = new Random();
			string nonceFromTheClient = collection["payment_method_nonce"];
			if (TempData["totalAmount"] != null)
			{
				int totalAmount = (int)TempData["totalAmount"];
				// If you need to convert it back to a decimal for the transaction request
				decimal totalAmountDecimal = Convert.ToDecimal(totalAmount);
                var request = new TransactionRequest
                {
                    Amount = totalAmountDecimal,
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId = "55501",
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };
                var gateway = _brain.GetGateway();
                Result<Transaction> result = gateway.Transaction.Sale(request);
                if (result.Target.ProcessorResponseText == "Approved")
                {
                    TempData["Success"] = "Transaction was successful Transaction ID "
                    + result.Target.Id + "Amount Charged : $" + result.Target.Amount;
                }
            }
            string url = string.Format("/Customer/Cart/OrderConfirmation");

            return Redirect(url);
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using System;
using Kclinic.Models;
using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace KclinicWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
    public class CoinPaymentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CoinPaymentsController(IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ProcessCheckout()
        {
            var model = new OrderModel
            {
                OrderId = Guid.NewGuid().ToString(),
                OrderTotal = 5,
                ProductName = "Programming C# for Beginners",
                FirstName = "John",
                LastName = "Smith",
                Email = "John@yourstore.com"
            };
            return View(model);
        }

        [HttpPost]
        public void ProcessCheckout(OrderModel orderModel)
        {
            var queryParameters = CreateQueryParameters(orderModel);

            var baseUrl = _configuration.GetSection("CoinPayments")["BaseUrl"];
            var redirectUrl = QueryHelpers.AddQueryString(baseUrl, queryParameters);

            _httpContextAccessor.HttpContext.Response.Redirect(redirectUrl);
        }

        public IActionResult SuccessHandler(string orderNumber)
        {
            ViewBag.OrderNumber = orderNumber;

            return View("PaymentResponse");
        }

        [HttpPost]
        public IActionResult IPNHandler()
        {
            byte[] parameters;
            using (var stream = new MemoryStream())
            {
                Request.Body.CopyTo(stream);
                parameters = stream.ToArray();
            }
            var strRequest = Encoding.ASCII.GetString(parameters);
            var ipnSecret = _configuration.GetSection("CoinPayments")["IpnSecret"];

            if (Helper.VerifyIpnResponse(strRequest, Request.Headers["hmac"], ipnSecret,
                out Dictionary<string, string> values))
            {
                values.TryGetValue("first_name", out string firstName);
                values.TryGetValue("last_name", out string lastName);
                values.TryGetValue("email", out string email);
                values.TryGetValue("amount1", out string amount1);
                values.TryGetValue("subtotal", out string subtotal);
                values.TryGetValue("status", out string status);
                values.TryGetValue("status_text", out string statusText);

                var newPaymentStatus = Helper.GetPaymentStatus(status, statusText);

                switch (newPaymentStatus)
                {
                    case PaymentStatus.Pending:
                        {
                            //TODO: update order status and use logging mechanism instead

                            //order is pending                            
                            Debug.WriteLine($"Order Status {newPaymentStatus}");
                        }
                        break;
                    case PaymentStatus.Authorized:
                        {
                            //order is authorized                            
                            Debug.WriteLine("Order is Authorized");
                        }
                        break;
                    case PaymentStatus.Paid:
                        {
                            //order is paid             
                            Debug.WriteLine("Order is Paid");
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Debug.WriteLine("Issue Occurred with CoinPayments IPN");
            }

            //nothing should be rendered to visitor
            return Content("");
        }

        public IActionResult CancelOrder()
        {
            return View("PaymentResponse");
        }

        private IDictionary<string, string> CreateQueryParameters(OrderModel orderModel)
        {
            //get store location
            var storeLocation = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            var queryParameters = new Dictionary<string, string>()
            {
                //ipn settings
                ["ipn_version"] = "1.0",
                ["cmd"] = "_pay_auto",
                ["ipn_type"] = "simple",
                ["ipn_mode"] = "hmac",
                ["merchant"] = _configuration.GetSection("CoinPayments")["MerchantId"],
                ["allow_extra"] = "0",
                ["currency"] = "USD",
                ["amountf"] = orderModel.OrderTotal.ToString("N2"),
                ["item_name"] = orderModel.ProductName,

                //IPN, success and cancel URL
                ["success_url"] = $"{storeLocation}/CoinPayments/SuccessHandler?orderNumber={orderModel.OrderId}", // order summery page
                ["ipn_url"] = $"{storeLocation}/CoinPayments/IPNHandler",
                ["cancel_url"] = $"{storeLocation}/CoinPayments/CancelOrder",

                //order identifier                
                ["custom"] = orderModel.OrderId,

                //billing info
                ["first_name"] = orderModel.FirstName,
                ["last_name"] = orderModel.LastName,
                ["email"] = orderModel.Email,

            };
            return queryParameters;
        }


    }
}

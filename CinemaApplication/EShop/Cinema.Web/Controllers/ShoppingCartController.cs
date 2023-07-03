using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using Cinema.Repository;
using Cinema.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cinema.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(this._shoppingCartService.getShoppingCartInfo(userId));
        }
        
        public IActionResult DeleteFromShoppingCart(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            this._shoppingCartService.deleteProductFromShoppingCart(userId, id);
            
            return RedirectToAction("Index", "ShoppingCart");
        }

        private Order Order()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.orderNow(userId);
            //if (result != null)
            //{
            //   //return RedirectToAction("Index", "ShoppingCart");
            //    return RedirectToAction("Details", "Order", new {id=result.Id });
            //}
            //return RedirectToAction("Index", "ShoppingCart");
            return result;
        }

        public IActionResult PayOrder(string stripeEmail, string stripeToken)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = this._shoppingCartService.getShoppingCartInfo(userId);

            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = (Convert.ToInt32(order.TotalPrice) * 100),
                Description = "Cinema Application Payment",
                Currency = "usd",
                Customer = customer.Id
            });

            if (charge.Status == "succeeded")
            {
                var result = this.Order();
                
                if (result != null)
                {
                    return RedirectToAction("Details", "Order", new { id = result.Id });
                }
                else
                {
                    return RedirectToAction("Index", "ShoppingCart");
                }
            }

            return RedirectToAction("Index", "ShoppingCart");
        }

        
    }
}

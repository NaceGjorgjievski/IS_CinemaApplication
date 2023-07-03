using Cinema.Domain.Identity;
using Cinema.Services.Interface;
using ExcelDataReader;
using GemBox.Document;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            _orderService = orderService;
        }

        public IActionResult Index(string id)
        {
            var allUserOrders = this._orderService.GetUserOrders(id);
            return View(allUserOrders);
        }



        public IActionResult Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = this._orderService.GetOrderDetails(id);
            var totalPrice = this._orderService.GetTotalPrice(order.Id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["TotalPrice"] = totalPrice;
            return View(order);
        }

        public IActionResult ExportToPDF(Guid id)
        {
            var order = this._orderService.GetOrderDetails(id);
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "OrderDetails.docx");

            var document = DocumentModel.Load(templatePath);
            var userNameSurname = order.User.FirstName + " " + order.User.LastName;
            if (string.IsNullOrEmpty(userNameSurname))
            {
                userNameSurname = order.User.Email;
            }
            document.Content.Replace("{{OrderId}}", order.Id.ToString());
            document.Content.Replace("{{UserNameSurname}}", userNameSurname);

            StringBuilder sb = new StringBuilder();

            var total = 0.0;

            foreach (var item in order.TicketsInOrder)
            {
                total += item.Quantity * item.OrderedTicket.Price;
                sb.AppendLine(item.OrderedTicket.MovieName+ " with quantity of: " + item.Quantity + " and price of: $" + item.OrderedTicket.Price);
            }

            document.Content.Replace("{{TicketList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", "$" + total.ToString());

            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());


            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "OrderDetails.pdf");
        }

      
    }
}

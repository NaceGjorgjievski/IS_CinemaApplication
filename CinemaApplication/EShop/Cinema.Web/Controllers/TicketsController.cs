
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Cinema.Services.Interface;
using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using ClosedXML.Excel;
using System.IO;

namespace Cinema.Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ICategoryService _categoryService;

        public TicketsController(ITicketService ticketService, ICategoryService categoryService)
        {
            _ticketService = ticketService;
            _categoryService = categoryService;
        }

        // GET: Tickets
        public IActionResult Index()
        {
            var time = Request.Query["startTIme"];
            var allProducts = this._ticketService.GetAllProducts();
            if (!string.IsNullOrEmpty(time))
            {
                DateTime startTime = DateTime.Parse(time);
                allProducts = allProducts.Where(z => z.MovieTime >= startTime).ToList();
            }
            ViewData["CategoryId"] = new SelectList(this._categoryService.GetAllProducts(), "Id", "CategoryName");
            return View(allProducts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Guid categoryId)
        {
            var category = this._categoryService.GetDetailsForProduct(categoryId);
            string fileName = category.CategoryName + "Tickets.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var result = this._ticketService.GetTicketsByCategoryId(categoryId);

            using (var workBook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workBook.Worksheets.Add("Tickets");

                worksheet.Cell(1, 1).Value = "Ticket Id";
                worksheet.Cell(1, 2).Value = "Movie Name";
                worksheet.Cell(1, 3).Value = "Movie Category";
                worksheet.Cell(1, 4).Value = "Movie Time";
                worksheet.Cell(1, 5).Value = "Price";

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.MovieName;
                    worksheet.Cell(i + 1, 3).Value = item.MovieCategory.CategoryName;
                    worksheet.Cell(i + 1, 4).Value = item.MovieTime;
                    worksheet.Cell(i + 1, 5).Value = item.Price;

                }

                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);

                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }
            }
        }

        public IActionResult AddTicketToCart(Guid? id)
        {
            var model = this._ticketService.GetShoppingCartInfo(id);
            ViewData["productImage"] = this._ticketService.GetDetailsForProduct(id).MovieImage;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTicketToCart([Bind("TicketId","Quantity")]AddToShoppingCartDto item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._ticketService.AddToShoppingCart(item, userId);

            if (result)
            {
                return RedirectToAction("Index", "Tickets");
            }
       
            return View(item);
        }

        // GET: Tickets/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForProduct(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            ViewData["CategoryId"] = new SelectList(this._categoryService.GetAllProducts(), "Id", "CategoryName");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,MovieName,MovieImage,CategoryId,MovieTime,Price")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                this._ticketService.CreateNewProduct(ticket);
                return RedirectToAction(nameof(Index));
            }
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", ticket.CategoryId);
            ViewData["CategoryId"] = new SelectList(this._categoryService.GetAllProducts(), "Id", "Id", ticket.CategoryId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForProduct(id);
            if (ticket == null)
            {
                return NotFound();
            }
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", ticket.CategoryId);
            ViewData["CategoryId"] = new SelectList(this._categoryService.GetAllProducts(), "Id", "CategoryName");
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,MovieName,MovieImage,CategoryId,MovieTime,Price")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this._ticketService.UpdeteExistingProduct(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", ticket.CategoryId);
            ViewData["CategoryId"] = new SelectList(this._categoryService.GetAllProducts(), "Id", "Id", ticket.CategoryId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForProduct(id);
              
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            this._ticketService.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(Guid id)
        {
            return this._ticketService.GetDetailsForProduct(id) != null;
        }
    }
}

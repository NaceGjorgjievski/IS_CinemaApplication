using Cinema.Domain.DomainModels;
using Cinema.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;
        string errorMessage = string.Empty;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }
        public List<Order> GetAllOrders()
        {
            return entities
                .Include(z => z.TicketsInOrder)
                .Include(z => z.User)
                .Include("TicketsInOrder.OrderedTicket")
                .ToListAsync().Result;
        }

        public Order GetOrderDetails(Guid id)
        {
            return entities
                .Include(z => z.TicketsInOrder)
                .Include(z => z.User)
                .Include("TicketsInOrder.OrderedTicket")
                .SingleOrDefaultAsync(z => z.Id == id).Result;
        }

        public int GetTotalPrice(Guid id)
        {
            int TotalPrice = 0;
            Order order = this.GetOrderDetails(id);
            foreach(var ticket in order.TicketsInOrder)
            {
                TotalPrice = TotalPrice + (ticket.Quantity * ticket.OrderedTicket.Price);
            }
            return TotalPrice;
        }

        public List<Order> GetUserOrders(string id)
        {
            List<Order> allOrders = this.GetAllOrders();
            List<Order> userOrders = new List<Order>();
            foreach(var order in allOrders)
            {
                if(order.UserId.Equals(id))
                {
                    userOrders.Add(order);
                }
            }
            return userOrders;
        }
    }
}

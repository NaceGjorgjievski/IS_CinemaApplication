using Cinema.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Services.Interface
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        Order GetOrderDetails(Guid id);
        int GetTotalPrice(Guid id);
        List<Order> GetUserOrders(string id);
    }
}

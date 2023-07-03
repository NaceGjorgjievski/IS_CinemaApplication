﻿using Cinema.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Repository.Interface
{
    public interface IOrderRepository
    {
        List<Order> GetAllOrders();
        List<Order> GetUserOrders(string id);
        Order GetOrderDetails(Guid id);

        int GetTotalPrice(Guid id);
    }
}

using Cinema.Domain.DomainModels;
using Cinema.Repository.Interface;
using Cinema.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<Order> GetAllOrders()
        {
            return this._orderRepository.GetAllOrders();
        }

        public Order GetOrderDetails(Guid id)
        {
            return this._orderRepository.GetOrderDetails(id);
        }

        public int GetTotalPrice(Guid id)
        {
            return this._orderRepository.GetTotalPrice(id);
        }

        public List<Order> GetUserOrders(string id)
        {
            return this._orderRepository.GetUserOrders(id);
        }
    }
}

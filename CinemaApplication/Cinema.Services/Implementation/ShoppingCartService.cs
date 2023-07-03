using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using Cinema.Repository.Interface;
using Cinema.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Services.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
        private readonly IRepository<EmailMessage> _emailMessagesRepository;
        private readonly IEmailService _emailService;
        public ShoppingCartService(IEmailService emailService, IRepository<EmailMessage> emailMessagesRepository, IRepository<ShoppingCart> shoppingCartRepository, IUserRepository userRepository, IRepository<Order> orderRepository, IRepository<TicketInOrder> ticketInOrderRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _ticketInOrderRepository = ticketInOrderRepository;
            _emailMessagesRepository = emailMessagesRepository;
            _emailService = emailService;
        }

        public void deleteProductFromShoppingCart(string userId, Guid id)
        {
            if (!string.IsNullOrEmpty(userId) && id != null)
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                var itemToDelete = userShoppingCart.TicketInShoppingCarts.Where(z => z.TicketId.Equals(id)).FirstOrDefault();

                userShoppingCart.TicketInShoppingCarts.Remove(itemToDelete);

                this._shoppingCartRepository.Update(userShoppingCart);
            }
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            var loggedInUser = this._userRepository.Get(userId);

            var userShoppingCart = loggedInUser.UserCart;

            var AllTickets = userShoppingCart.TicketInShoppingCarts.ToList();

            var allTicketPrices = AllTickets.Select(z => new
            {
                TicketPrice = z.Ticket.Price,
                Quantity = z.Quantity
            }).ToList();

            var totalPrice = 0;

            foreach (var item in allTicketPrices)
            {
                totalPrice += item.Quantity * item.TicketPrice;
            }

            ShoppingCartDto scDto = new ShoppingCartDto
            {
                Tickets = AllTickets,
                TotalPrice = totalPrice
            };

            return scDto;
        }

        public Order orderNow(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                var AllTickets = userShoppingCart.TicketInShoppingCarts.ToList();

                var allTicketPrices = AllTickets.Select(z => new
                {
                    TicketPrice = z.Ticket.Price,
                    Quantity = z.Quantity
                }).ToList();

                var totalPrice = 0;

                foreach (var item in allTicketPrices)
                {
                    totalPrice += item.Quantity * item.TicketPrice;
                }

                EmailMessage message = new EmailMessage();
                message.MailTo = loggedInUser.Email;
                message.Subject = "Succesfully Created Order";
                message.Status = false;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Your order is completed. The order conatins: ");
                for(int i = 1; i <= AllTickets.Count(); i++)
                {
                    var currentItem = AllTickets[i - 1];
                    sb.AppendLine(i.ToString() + ". " + currentItem.Ticket.MovieName + " with quantity of " + currentItem.Quantity + " and price of $" + currentItem.Ticket.Price);
                }

                sb.AppendLine("Total Price of your order: $" + totalPrice.ToString());

                message.Content = sb.ToString();

                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = loggedInUser,
                    UserId = userId,
                    TotalPrice = totalPrice
                };

                this._orderRepository.Insert(order);

                List<TicketInOrder> ticketInOrders = new List<TicketInOrder>();

                var result = userShoppingCart.TicketInShoppingCarts.Select(z => new TicketInOrder
                {
                    Id = Guid.NewGuid(),
                    TicketId = z.Ticket.Id,
                    OrderedTicket = z.Ticket,
                    OrderId = order.Id,
                    UserOrder = order,
                    Quantity = z.Quantity
                }).ToList();

                ticketInOrders.AddRange(result);

                foreach (var item in ticketInOrders)
                {
                    this._ticketInOrderRepository.Insert(item);
                }
                
                loggedInUser.UserCart.TicketInShoppingCarts.Clear();
                
                this._userRepository.Update(loggedInUser);
                this._emailMessagesRepository.Insert(message);
                this.sendEmail();
                
                
                return order;
            }
            return null;
        }

        private async Task sendEmail()
        {
            await _emailService.SendEmailAsync(this._emailMessagesRepository.GetAll().ToList());
            var emails = this._emailMessagesRepository.GetAll();
            foreach(var mail in emails)
            {
                _emailMessagesRepository.Delete(mail);
            }
        }
    }
}

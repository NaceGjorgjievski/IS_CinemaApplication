using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using Cinema.Repository.Interface;
using Cinema.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Services.Implementation
{
    public class TicketService : ITicketService
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<TicketInShoppingCart> _ticketInShoppingCartRepository;
        public TicketService(IRepository<Ticket> ticketRepository, IUserRepository userRepository, IRepository<TicketInShoppingCart> ticketInShoppingCartRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _ticketInShoppingCartRepository = ticketInShoppingCartRepository;
        }

        public bool AddToShoppingCart(AddToShoppingCartDto item, string userID)
        {
            var user = this._userRepository.Get(userID);
            var userShoppingCart = user.UserCart;

            if (item.TicketId != null && userShoppingCart != null)
            {
                var ticket = this.GetDetailsForProduct(item.TicketId);

                if (ticket != null)
                {
                    TicketInShoppingCart itemToAdd = new TicketInShoppingCart
                    {
                        Id = Guid.NewGuid(),
                        Ticket = ticket,
                        TicketId = ticket.Id,
                        ShoppingCart = userShoppingCart,
                        ShoppingCartId = userShoppingCart.Id,
                        Quantity = item.Quantity
                    };

                    this._ticketInShoppingCartRepository.Insert(itemToAdd);
                    return true;
                }
                return false;
            }
            return false;
        }

        public void CreateNewProduct(Ticket p)
        {
            this._ticketRepository.Insert(p);
        }

        public void DeleteProduct(Guid id)
        {
            var ticket = this.GetDetailsForProduct(id);
            this._ticketRepository.Delete(ticket);
        }

        public List<Ticket> GetAllProducts()
        {
           return this._ticketRepository.GetAll().ToList();
        }

        public Ticket GetDetailsForProduct(Guid? id)
        {
            return this._ticketRepository.Get(id);
        }

        public AddToShoppingCartDto GetShoppingCartInfo(Guid? id)
        {
            var ticket = this.GetDetailsForProduct(id);
            AddToShoppingCartDto model = new AddToShoppingCartDto
            {
                SelectedTicket = ticket,
                TicketId = ticket.Id,
                Quantity = 1
            };
            return model;
        }

        public List<Ticket> GetTicketsByCategoryId(Guid id)
        {
            var allTickets = this.GetAllProducts();
            List<Ticket> ticketsByCategory = new List<Ticket>();
            foreach(var ticket in allTickets)
            {
                if (ticket.CategoryId.Equals(id))
                {
                    ticketsByCategory.Add(ticket);
                }
            }
            return ticketsByCategory;
        }

        public void UpdeteExistingProduct(Ticket p)
        {
            this._ticketRepository.Update(p);
        }
    }
}

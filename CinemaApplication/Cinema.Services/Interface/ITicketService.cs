using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Services.Interface
{
    public interface ITicketService
    {
        List<Ticket> GetAllProducts();
        Ticket GetDetailsForProduct(Guid? id);
        void CreateNewProduct(Ticket p);
        void UpdeteExistingProduct(Ticket p);
        AddToShoppingCartDto GetShoppingCartInfo(Guid? id);
        void DeleteProduct(Guid id);
        bool AddToShoppingCart(AddToShoppingCartDto item, string userID);

        List<Ticket> GetTicketsByCategoryId(Guid id);
    }
}

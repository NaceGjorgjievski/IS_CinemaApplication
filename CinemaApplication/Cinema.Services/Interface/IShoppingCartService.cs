using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Services.Interface
{
    public interface IShoppingCartService
    {
        ShoppingCartDto getShoppingCartInfo(string userId);
        void deleteProductFromShoppingCart(string userId,Guid id);

        Order orderNow(string userId);
    }
}

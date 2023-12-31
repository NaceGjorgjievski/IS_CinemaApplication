﻿using Cinema.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Domain.DTO
{
    public class ShoppingCartDto
    {
        public List<TicketInShoppingCart> Tickets { get; set; }
        public double TotalPrice { get; set; }
    }
}

using Cinema.Domain.DomainModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Domain.Identity
{
    public class CinemaApplicationUser : IdentityUser
    {
        public String FirstName{ get; set; }
        public String LastName { get; set; }

        public virtual ShoppingCart UserCart { get; set; }

        public virtual ICollection<Order> Orders{ get; set; }

    }
}

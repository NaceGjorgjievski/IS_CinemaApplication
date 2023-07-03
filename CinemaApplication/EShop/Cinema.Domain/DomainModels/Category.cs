using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Domain.DomainModels
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Domain.DomainModels
{
    public class Ticket : BaseEntity
    {
        [Required]
        public string MovieName { get; set; }
        [Required]
        public string MovieImage { get; set; }
        public Guid CategoryId { get; set; }
        public Category MovieCategory { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime MovieTime{ get; set; }
        [Required]
        public int Price { get; set; }

        public virtual ICollection<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public IEnumerable<TicketInOrder> TicketsInOrder { get; set; }
    }
}

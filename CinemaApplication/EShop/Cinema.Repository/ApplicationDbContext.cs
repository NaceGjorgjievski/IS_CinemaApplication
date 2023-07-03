using Cinema.Domain.DomainModels;
using Cinema.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Cinema.Repository
{
    public class ApplicationDbContext : IdentityDbContext<CinemaApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public virtual DbSet<TicketInOrder> TicketInOrders { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<EmailMessage> EmailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Ticket>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ShoppingCart>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Category>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

        //    builder.Entity<TicketInShoppingCart>()
        //        .HasKey(z => new { z.TicketId, z.ShoppingCartId });

            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.TicketInShoppingCarts)
                .HasForeignKey(z => z.ShoppingCartId);

            builder.Entity<TicketInShoppingCart>()
              .HasOne(z => z.ShoppingCart)
              .WithMany(z => z.TicketInShoppingCarts)
              .HasForeignKey(z => z.TicketId);

            builder.Entity<Ticket>()
                .HasOne<Category>(z => z.MovieCategory)
                .WithMany(z => z.Tickets)
                .HasForeignKey(z => z.CategoryId);

            builder.Entity<ShoppingCart>()
                .HasOne<CinemaApplicationUser>(z => z.Owner)
                .WithOne(z => z.UserCart)
                .HasForeignKey<ShoppingCart>(z => z.OwnerId);

        //    builder.Entity<TicketInOrder>()
        //        .HasKey(z => new { z.TicketId, z.OrderId });

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.OrderedTicket)
                .WithMany(z => z.TicketsInOrder)
                .HasForeignKey(z => z.TicketId);

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.UserOrder)
                .WithMany(z => z.TicketsInOrder)
                .HasForeignKey(z => z.OrderId);
        }
    }
}

using Cinema.Domain.Identity;
using Cinema.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<CinemaApplicationUser> entities;
        string errorMessage = string.Empty;
        
        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<CinemaApplicationUser>();
        }
        public IEnumerable<CinemaApplicationUser> GetAll()
        {
            return entities.AsEnumerable();
        }

        public CinemaApplicationUser Get(string id)
        {
            return entities
                .Include(z => z.UserCart)
                .Include("UserCart.TicketInShoppingCarts")
                .Include("UserCart.TicketInShoppingCarts.Ticket")
                .SingleOrDefault(s => s.Id == id);
        }
        public void Insert(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }

        public string GetUserRole(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            return context.UserClaims.Where(z=> z.UserId.Equals(entity.Id)).FirstOrDefault().ClaimValue;
        }

        public void SetAdmin(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            var userClaim = context.UserClaims.Where(z => z.UserId.Equals(entity.Id)).FirstOrDefault();
            userClaim.ClaimValue = "Admin";
            context.UserClaims.Update(userClaim);
            context.SaveChanges();
        }

        public void SetStandardUser(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            var userClaim = context.UserClaims.Where(z => z.UserId.Equals(entity.Id)).FirstOrDefault();
            userClaim.ClaimValue = "StandardUser";
            context.UserClaims.Update(userClaim);
            context.SaveChanges();
        }
    }
}

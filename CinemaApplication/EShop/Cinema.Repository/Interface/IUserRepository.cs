using Cinema.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<CinemaApplicationUser> GetAll();
        CinemaApplicationUser Get(string id);
        void Insert(CinemaApplicationUser entity);
        void Update(CinemaApplicationUser entity);
        void Delete(CinemaApplicationUser entity);
        string GetUserRole(CinemaApplicationUser entity);
        void SetAdmin(CinemaApplicationUser entity);
        void SetStandardUser(CinemaApplicationUser entity);
    }
}

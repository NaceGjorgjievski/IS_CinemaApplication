using Cinema.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Services.Interface
{
    public interface IUserService
    {
        List<CinemaApplicationUser> getAllUsers();
        CinemaApplicationUser getUser(string id);
        string GetUserRole(CinemaApplicationUser user);
        void SetAdmin(string id);
        void SetStandardUser(string id);
        void RemoveUser(string id);
    }
}

using Cinema.Domain.Identity;
using Cinema.Repository.Interface;
using Cinema.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public List<CinemaApplicationUser> getAllUsers()
        {
            return _userRepository.GetAll().ToList();
        }

        public string GetUserRole(CinemaApplicationUser user)
        {
            return this._userRepository.GetUserRole(user);
        }

        public CinemaApplicationUser getUser(string id)
        {
            return this._userRepository.Get(id);
        }

        public void SetAdmin(string id)
        {
            var entity = this._userRepository.Get(id);
            this._userRepository.SetAdmin(entity);
        }

        public void SetStandardUser(string id)
        {
            var entity = this._userRepository.Get(id);
            this._userRepository.SetStandardUser(entity);
        }

        public void RemoveUser(string id)
        {
            var entity = this._userRepository.Get(id);
            this._userRepository.Delete(entity);
        }
    }
}

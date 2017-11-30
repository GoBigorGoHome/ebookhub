using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using ebookhub.Models;

namespace ebookhub.Data
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task UpdateUser(User user);
        Task AddUser(User user);
    }
}
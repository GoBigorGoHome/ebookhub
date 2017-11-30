using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ebookhub.Models;

namespace ebookhub.Data
{
    public class UserRepository : IUserRepository
    {
        private UserContext _context;

        public UserRepository(IOptions<DatabaseOptions> options)
        {
            _context = new UserContext(options);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.Find(_ => true).ToListAsync();
        }

        public async Task UpdateUser(User user)
        {
            var existingUser = await _context.Users.FindAsync(user1 => user1.Id == user.Id);
            if (existingUser.FirstOrDefault() == null)
            {
                await _context.Users.InsertOneAsync(user);
                return;
            }

            await _context.Users
                .ReplaceOneAsync(b => b.Id.Equals(user.Id)
                    , user
                    , new UpdateOptions { IsUpsert = true });
        }

        public async Task AddUser(User user)
        {
            await _context.Users.InsertOneAsync(user);
        }
    }
}

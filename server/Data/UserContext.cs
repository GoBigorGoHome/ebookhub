using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ebookhub.Models;

namespace ebookhub.Data
{
    public class UserContext
    {
        private readonly IMongoDatabase _database = null;
        public UserContext(IOptions<DatabaseOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            _database = client.GetDatabase(options.Value.Database);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    }
}

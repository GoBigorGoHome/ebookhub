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
    public class BookContext
    {
        private readonly IMongoDatabase _database = null;

        public BookContext(IOptions<DatabaseOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(options.Value.Database);
        }
        public IMongoCollection<Book> Books
        {
            get
            {
                return _database.GetCollection<Book>("Books");
            }
        }
    }
}

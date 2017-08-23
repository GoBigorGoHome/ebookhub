using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using ebookhub.Models;

namespace ebookhub.Data
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooks();
        Task<Book> GetBookById(ObjectId id);
        Task InsertManyBooks(IEnumerable<Book> books);
        Task<IEnumerable<Book>> SearchBook(string searchTerm);
        Book UpdateFilePath(Book book, string path);
        bool IsBookExisting(string filePath);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ebookhub.Models;

namespace ebookhub.Data
{
    public class BookRepository : IBookRepository
    {
        private readonly BookContext _context = null;

        public BookRepository(IOptions<DatabaseOptions> options)
        {
            _context = new BookContext(options);
        }
        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            //var test = new ObjectId();
            return await _context.Books.Find(_ => true).ToListAsync();
        }

        public async Task<Book> GetBookById(ObjectId id)
        {
            return await _context.Books.Find(b => b.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Book>> SearchBook(string searchTerm)
        {
            var query = $"{{ Title: {{ $regex: '{searchTerm}', $options: 'i' }} }}";
            return await _context.Books.Aggregate()
            .Match(query).ToListAsync();
        }

        public async Task InsertManyBooks(IEnumerable<Book> books)
        {
            await _context.Books.InsertManyAsync(books);
        }

        public Book UpdateFilePath(Book book, string path)
        {
            var ex = Path.GetExtension(path).Replace(".", "");
            if(ex == FileType.epub.ToString() && !book.Files.Any(f => f.FileType == FileType.epub))
            {
                book.Files.Add(new EBookFile(){FileType = FileType.epub, RelativeFilePath = path});
            }
            if(ex == FileType.mobi.ToString() && !book.Files.Any(f => f.FileType == FileType.mobi))
            {
                book.Files.Add(new EBookFile(){FileType = FileType.mobi, RelativeFilePath = path});
            }

            _context.Books
                .ReplaceOne(b => b.Id.Equals(book.Id)
                        , book
                        , new UpdateOptions { IsUpsert = true });

            return book;
        }

        public bool IsFileExisting(string filePath)
        {
            return _context.Books.Find(b => b.Files.Any(f => f.RelativeFilePath == filePath)).Any();
        }
    }
}
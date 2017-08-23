using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Clauses;
using ebookhub.Data;
using ebookhub.Models;

namespace ebookhub.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly IBookRepository _repository;
        public SearchController(IBookRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> FreeSearch(string searchTerm)
        {
            var books = await _repository.SearchBook(searchTerm);

            return books.Select(book => book.Title).ToList();
        }

        [HttpGet("books")]
        public async Task<IEnumerable<Book>> SearchBooks(string searchTerm)
        {
            return await _repository.SearchBook(searchTerm);
        }
    }
}

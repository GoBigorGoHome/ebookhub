using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using ebookhub.Calibre;
using ebookhub.Data;
using ebookhub.Models;

namespace ebookhub.Controllers
{
    [Route("api/[controller]")]
    public class BooksController
    {
        private readonly IBookRepository _repository;
        private readonly IUserRepository _userRepo;
        private readonly ILogger _logger;
        private CalibreOptions _calibreOptions;
        private SmtpOptions _smtpOptions;

        public BooksController(IBookRepository repository, 
            IUserRepository userRepo,
            ILogger<ImportController> logger, 
            IOptions<CalibreOptions> options,
            IOptions<SmtpOptions> smtpOptions)
        {
            _repository = repository;
            _userRepo = userRepo;
            _logger = logger;
            _calibreOptions = options.Value;
            _smtpOptions = smtpOptions.Value;
        }

        [HttpGet("")]
        public async Task<IEnumerable<Book>> Get()
        {
            return await _repository.GetAllBooks();
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send(string bookID)
        {
            _logger.LogDebug("Sending book");

            var book = await _repository.GetBookById(new ObjectId(bookID));

            if(book == null)
            {
                return new NotFoundResult();
            }


            BackgroundJob.Enqueue(() => DoSendBook(bookID));

            return new OkResult();
        }

        [HttpGet("dummy")]
        public void DoSendBook(string bookId)
        {
            var book = _repository.GetBookById(new ObjectId(bookId)).Result;

            if (book == null)
            {
                return;
            }

            EBookFile file = null;
            if (book.Files.Count > 0)
            {
                file = book.Files.FirstOrDefault(b => b.FileType == FileType.mobi);
                if (file == null)
                {
                    var calibre = new Calibre.Calibre(_repository, _logger, Options.Create(_calibreOptions));
                    var convertedFile = calibre.ConvertBookToMobi(
                        Path.Combine(_calibreOptions.ContentFolder, book.Files.First().RelativeFilePath));

                    var convertedFileRelativePath = convertedFile.Result;
                    if (convertedFileRelativePath.StartsWith(_calibreOptions.ContentFolder))
                    {
                        convertedFileRelativePath = convertedFileRelativePath.Substring(_calibreOptions.ContentFolder.Length);
                        while (convertedFileRelativePath.IndexOf(Path.DirectorySeparatorChar) == 0
                               || convertedFileRelativePath.IndexOf(Path.AltDirectorySeparatorChar) == 0)
                        {
                            convertedFileRelativePath = convertedFileRelativePath.Substring(1);
                        }
                    }

                    book = _repository.UpdateFilePath(book, convertedFileRelativePath);
                }
            }

            file = book.Files.FirstOrDefault(b => b.FileType == FileType.mobi);
            if (file == null)
            {
                throw new IOException();
            }

            var sender = new MailSender(Options.Create(_smtpOptions), Options.Create(_calibreOptions));
            sender.SendFileByMail(file, _userRepo.GetAllUsers().Result.FirstOrDefault());
        }
    }
}

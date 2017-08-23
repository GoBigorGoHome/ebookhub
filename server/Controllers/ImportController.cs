using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ebookhub.Calibre;
using ebookhub.Models;
using Microsoft.Extensions.Logging;
using ebookhub.Data;

namespace ebookhub.Controllers
{
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        private readonly IBookRepository _repository;
        private readonly ILogger _logger;
        private CalibreOptions _options;
        public ImportController(IBookRepository repository, ILogger<ImportController> logger, IOptions<CalibreOptions> options)
        {
            this._repository = repository;
            _logger = logger;
            _options = options.Value;
        }

        [HttpGet("all")]
        public ActionResult Get()
        {
            _logger.LogDebug("Import all books");
            //Calibre.Calibre calibreProcess = new Calibre.Calibre(_logger, _options.CalibreBinFolder);
            //var books = await calibreProcess.ImportFromFolder(_options.ContentFolder, _options.FileTypes);
            //await _repository.InsertManyBooks(books);
            BackgroundJob.Enqueue(() => DoImport());
            return new OkResult();
        }

        public void DoImport()
        {
            Calibre.Calibre calibreProcess = new Calibre.Calibre(_repository, _logger, Options.Create(_options));
            var books = calibreProcess.ImportFromFolder(_options.ContentFolder, _options.FileTypes).Result;
            //_repository.InsertManyBooks(books);
        }
    }
}

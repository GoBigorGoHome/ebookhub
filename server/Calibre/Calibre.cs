using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ebookhub.Data;
using ebookhub.Models;

namespace ebookhub.Calibre
{
    class CalibreProcess
    {
        private static List<string> calibreOutput = new List<string>();
        private static int numOutputLines = 0;

        public static List<string> ImportBook(string calibreExecutable, string fileName)
        {
            calibreOutput = new List<string>();
            numOutputLines = 0;
            var executable = calibreExecutable;
            var arguments = $"\"{fileName}\"";
            return RunCommand(executable, arguments);
        }

        public static List<string> ConvertBook(string calibreExecutable, string input, string output)
        {
            calibreOutput = new List<string>();
            numOutputLines = 0;
            var executable = calibreExecutable;
            var arguments = $"\"{input}\" \"{output}\"";
            return RunCommand(executable, arguments);
        }

        private static List<string> RunCommand(string executable, string arguments)
        {
            var calibreProcess = new Process();
            calibreProcess.StartInfo.FileName = executable;
            calibreProcess.StartInfo.Arguments = arguments;

            // Set UseShellExecute to false for redirection.
            calibreProcess.StartInfo.UseShellExecute = false;

            // Redirect the standard output of the sort command.  
            // This stream is read asynchronously using an event handler.
            calibreProcess.StartInfo.RedirectStandardOutput = true;

            // Set our event handler to asynchronously read the sort output.
            calibreProcess.OutputDataReceived += new DataReceivedEventHandler(CalibreOutputHandler);

            // Redirect standard input as well.  This stream
            // is used synchronously.
            calibreProcess.StartInfo.RedirectStandardInput = true;

            // Start the process.
            calibreProcess.Start();

            // Use a stream writer to synchronously write the sort input.
            StreamWriter sortStreamWriter = calibreProcess.StandardInput;

            // Start the asynchronous read of the sort output stream.
            calibreProcess.BeginOutputReadLine();

            // Wait for the sort process to write the sorted text lines.
            calibreProcess.WaitForExit();

            return calibreOutput;
        }

        private static void CalibreOutputHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            // Collect the sort command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                numOutputLines++;

                // Add the text to the collected output.
                calibreOutput.Add(outLine.Data);
            }
        }
    }

    public class Calibre
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger _logger;
        private readonly CalibreOptions _options;
        private Dictionary<string, string> _calibreToBookProperty = new Dictionary<string, string>
        {
            { "Title", "Title" },
            { "Author(s)", "Authors" },
            { "Tags", "Tags" },
            { "Published", "Published" },
            { "Identifiers", "Identifiers" }
        };
        public Calibre(IBookRepository bookRepository, ILogger logger, IOptions<CalibreOptions> options)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _options = options.Value;
        }

        private async Task<Book> Import(string fileName)
        {
            var task = Task.Run(() => CalibreProcess.ImportBook(Path.Combine(_options.CalibreBinFolder, "ebook-meta"), fileName));
            await task;

            Dictionary<string, string> bookValues = new Dictionary<string, string>();
            foreach (var line in task.Result)
            {
                List<string> splitLine = line.Split(':').ToList();
                if (splitLine.Count >= 2)
                {
                    string propName;
                    if (_calibreToBookProperty.TryGetValue(splitLine[0].Trim(), out propName))
                    {
                        bookValues[propName] = String.Join(":", splitLine.GetRange(1, splitLine.Count - 1)).Trim();
                    }
                }
            }

            if (bookValues.ContainsKey("Title"))
            {
                Book book = new Book() {Title = bookValues["Title"]};

                if (bookValues.ContainsKey("Authors"))
                {
                    var authors = bookValues["Authors"];
                    int i = authors.IndexOf('[');
                    if (i > 0)
                    {
                        authors = authors.Remove(i);
                    }
                    foreach (var authorName in authors.Split(';'))
                    {
                        foreach (var cleanAuthorName in authorName.Split('&'))
                        {
                            Author author = new Author() {FullName = cleanAuthorName.Trim()};
                            book.Authors.Add(author);
                        }
                    }
                }

                if (bookValues.ContainsKey("Tags"))
                {
                    foreach (var tagValue in bookValues["Tags"].Split(';'))
                    {
                        book.Tags.Add(new Tag() { Name = tagValue.Trim() });
                    } 
                }

                if (bookValues.ContainsKey("Identifiers"))
                {
                    List<string> ids = bookValues["Identifiers"].Split(',').ToList();
                    book.Identifiers = ids;
                }

                if (bookValues.ContainsKey("Published"))
                {
                    DateTime pubDate = new DateTime();
                    var success = DateTime.TryParse(bookValues["Published"], out pubDate);
                    if (success)
                    {
                        book.PublishDateTime = pubDate;
                    }
                }

                _logger.LogInformation($"Imported: {book.Title}");
                return book;
            }
            
            return null;
        }

        public async Task ImportFromFolder(string folder, List<string> fileTypes)
        {
            List<string> foundFiles = new List<string>();
            foreach (var fileType in fileTypes)
            {
                foundFiles.AddRange(Directory.EnumerateFiles(folder, fileType, SearchOption.AllDirectories));
            }

            _logger.LogInformation($"Found {foundFiles.Count} files to import.");

            string fullPath = Path.GetFullPath(folder);

            int counter = 0;
            foreach (string foundFile in foundFiles)
            {
                counter++;
                _logger.LogInformation($"{counter}/{foundFiles.Count} - {foundFile}");

                var relPath = ExtractRelativePath(foundFile, _options.ContentFolder);
                if(_bookRepository.IsFileExisting(relPath))
                {
                    _logger.LogInformation($"File has already been imported - {foundFile}");
                    continue;
                }

                Book book = await Import(foundFile);
                if (book != null)
                {
                    book.Files.Add(new EBookFile()
                    {
                        FileType = GetFileType(relPath),
                        RelativeFilePath = relPath
                    });

                    var existingBook = _bookRepository.GetBookByTitle(book.Title);
                    if (existingBook != null)
                    {
                        if (existingBook.Files.All(file => file.RelativeFilePath != relPath))
                        {
                            _bookRepository.UpdateFilePath(existingBook, relPath);
                        }
                    }
                    else
                    {
                        await _bookRepository.InsertManyBooks(new List<Book>(){book});
                    }
                }
            }
        }

        public async Task<string> ConvertBookToMobi(string fullPath)
        {
            string output_path = Path.ChangeExtension(fullPath, "mobi");
            if (File.Exists(output_path))
            {
                _logger.LogWarning("Output file already exists and will be replaced");
                File.Delete(output_path);
            }
            var task = Task.Run(() => CalibreProcess.ConvertBook(
                Path.Combine(_options.CalibreBinFolder, "ebook-convert"), fullPath, output_path));
            await task;

            if (!File.Exists(output_path))
            {
                _logger.LogError("Coversion failed!");
                return null;
            }

            return output_path;
        }

        private FileType GetFileType(string path)
        {
            var ex = Path.GetExtension(path).Replace(".", "");
            if (ex == FileType.epub.ToString())
            {
                return FileType.epub;
            }
            if (ex == FileType.mobi.ToString())
            {
                return FileType.mobi;
            }
            return FileType.unknown;
        }

        private string ExtractRelativePath(string fullPath, string prefixPath)
        {
            var relPath = fullPath;
            if (fullPath.StartsWith(prefixPath))
            {
                relPath = fullPath.Substring(prefixPath.Length);
                while (relPath.IndexOf(Path.DirectorySeparatorChar) == 0 
                    || relPath.IndexOf(Path.AltDirectorySeparatorChar) == 0)
                {
                    relPath = relPath.Substring(1);
                }
            }
            return relPath;
        }
    }
}

using System;
using System.Threading.Tasks;
using EfEagerLoad.ConsoleTester.Extensions;
using EfEagerLoad.ConsoleTester.Logging;
using EfEagerLoad.ConsoleTester.Model;
using EfEagerLoad.ConsoleTester.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfEagerLoad.ConsoleTester
{
    public class TestRunner
    {
        private readonly IRepository _repository;
        private readonly ILogger<TestRunner> _logger;

        public TestRunner(IRepository repository, ILogger<TestRunner>  logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task RunTest1()
        {
            _logger.LogInformation("___________________________ Starting ___________________________");

            var books = await _repository.GetAll<Book>();
            foreach (var book in books)
            {
                _logger.LogInformation($"{book.Id} : {book.Name} : {book.Category.Name} : {book.Author.Name} : {book.Publisher.Name}");
                _logger.LogInformation("----------------------------------------");
            }

            Console.WriteLine("_______________ Finished Test Run _______________");
            _logger.LogError($"Commands: {ConsoleLogger.CommandCounter}");
        }

        public async Task RunTest2()
        {
            _logger.LogInformation("___________________________ Starting ___________________________");

            var authors = await _repository.GetAll<Author>();

            foreach (var author in authors)
            {
                _logger.LogInformation($"{author.Id} : {author.Name} : {author.Books.Count}");
                author.Books.ForEach(book =>
                {
                    _logger.LogInformation($"BOOK: {book.Id} : {book.Name} : {book.Category.Name} : {book.Publisher.Name}");
                });
                _logger.LogInformation("---------------------------");
            }

            Console.WriteLine("_______________ Finished Test Run _______________");
            _logger.LogError($"Commands: {ConsoleLogger.CommandCounter}");
        }

        public async Task RunTest3()
        {
            _logger.LogInformation("___________________________ Starting ___________________________");

            var pubs = await _repository.Query<Publisher>(eagerLoad: true)
                .Include(p => p.Books).ThenInclude((Book b) => b.Author).ThenInclude((a) => a.Books).ToListAsync();

            Console.WriteLine("_______________ Finished Test Run _______________");
            _logger.LogError($"Commands: {ConsoleLogger.CommandCounter}");
        } 
    }
}

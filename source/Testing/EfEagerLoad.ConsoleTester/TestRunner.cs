using System;
using System.Linq;
using System.Threading.Tasks;
using EfEagerLoad.ConsoleTester.Logging;
using EfEagerLoad.Testing.Extensions;
using EfEagerLoad.Testing.Model;
using EfEagerLoad.Testing.Repository;
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

            var publishers = await _repository.Query<Publisher>(eagerLoad: true).ToListAsync();

            foreach (var publisher in publishers)
            {
                _logger.LogInformation($"{publisher.Id} : {publisher.Name} : {publisher.Books.Count}");
                publisher.Books.ForEach(book =>
                {
                    _logger.LogInformation($"Book: {book.Id} : {book.Name} : {book.Category.Name} : {book.Publisher.Name}");
                });
                _logger.LogInformation("---------------------------");
            }

            Console.WriteLine("_______________ Finished Test Run _______________");
            _logger.LogError($"Commands: {ConsoleLogger.CommandCounter}");
        }

        public async Task RunTest4()
        {
            _logger.LogInformation("___________________________ Starting ___________________________");

            var countries = await _repository.Query<Country>(eagerLoad: true).ToListAsync();

            foreach (var country in countries)
            {
                _logger.LogInformation($"{country.Id} : {country.Name} : {country.Publishers.Count}");
                country.Publishers.ForEach(publisher =>
                {
                    _logger.LogInformation($"Publisher: {publisher.Id} : {publisher.Name} : {publisher.Books.Count}");
                });
                _logger.LogInformation("---------------------------");
            }

            Console.WriteLine("_______________ Finished Test Run _______________");
            _logger.LogError($"Commands: {ConsoleLogger.CommandCounter}");
        }

        public async Task<object> RunTest5()
        {
            _logger.LogInformation("___________________________ Starting ___________________________");

            return _repository.Query<Book>().Include(book => book.Author).ThenInclude(author => author.Books)
                .Include(book => book.Category)
                .Include(book => book.Publisher).ToList();
        }

        public async Task<object> RunTest6()
        {
            _logger.LogInformation("___________________________ Starting ___________________________");

            return await _repository.GetAll<Book>();
        }
    }
}

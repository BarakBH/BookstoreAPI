using BookstoreApi.Models;

namespace BookstoreApi.Services;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync();
    Task<Book?> GetByIsbnAsync(string isbn);
    Task AddAsync(Book book);
    Task UpdateAsync(string isbn, Book updated);
    Task DeleteAsync(string isbn);
}

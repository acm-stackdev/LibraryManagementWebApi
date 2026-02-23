using LibraryManagementSystem.Models;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<Book> CreateBookAsync(Book book, List<string> authorNames);
    Task UpdateBookAsync(Book book, List<string> authorNames);
    Task DeleteBookAsync(int id);
}
using LibraryManagementSystem.Models;

public interface IBookAuthorService
{
    Task<IEnumerable<BookAuthor>> GetAllAsync();
    Task<BookAuthor?> GetByIdAsync(int id);
    Task<BookAuthor> CreateAsync(BookAuthor bookAuthor);
    Task DeleteAsync(int id);
}
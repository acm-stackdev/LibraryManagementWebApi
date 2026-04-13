using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;

public interface IBookService
{
    Task<IEnumerable<BookDTO>> GetAllAsync();
    Task<BookDTO?> GetByIdAsync(int id);
    Task<Book?> GetByISBNAsync(string isbn);
    Task<BookDTO> CreateBookAsync(Book book, List<string> authorNames);
    Task UpdateBookAsync(int id, UpdateBookDTO dto);
    Task DeleteBookAsync(int id);
}
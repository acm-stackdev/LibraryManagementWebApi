using LibraryManagementSystem.Models;

public interface IAuthorService
{
    Task<IEnumerable<Author>> GetAllAsync();
    Task<Author?> GetByIdAsync(int id);
    Task<Author> CreateAsync(Author author);
    Task UpdateAsync(Author author);
    Task DeleteAsync(int id);
}
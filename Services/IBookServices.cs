using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendApi.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDTO>> GetAllAsync();
        Task<BookDTO?> GetByIdAsync(int id);
        Task<BookDTO?> GetByISBNAsync(string isbn);
        Task<BookDTO> CreateBookAsync(CreateBookDTO dto);
        Task<BookDTO?> UpdateBookAsync(int id, UpdateBookDTO dto);
        Task DeleteBookAsync(int id);
    }
}
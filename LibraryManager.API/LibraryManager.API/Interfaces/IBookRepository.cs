using LibraryManager.API.Models;

namespace LibraryManager.API.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync(bool withNoTracking = true, string[]? includes = null, CancellationToken cancellationToken = default);
        Task<Book?> GetByIdAsync(int id, string[]? includes = null, CancellationToken cancellationToken = default);
        Task AddAsync(Book book, CancellationToken cancellationToken = default);
        Task UpdateAsync(Book book, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}

using LibraryManager.API.Models;

namespace LibraryManager.API.Interfaces
{
    public interface IAuthorRepository
    {
        Task<List<Author>> GetAllAsync(bool withNoTracking = true, string[]? includes = null, CancellationToken cancellationToken = default);
        Task<Author?> GetByIdAsync(int id, string[]? includes = null, CancellationToken cancellationToken = default);
        Task AddAsync(Author author, CancellationToken cancellationToken = default);
        Task UpdateAsync(Author author, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}

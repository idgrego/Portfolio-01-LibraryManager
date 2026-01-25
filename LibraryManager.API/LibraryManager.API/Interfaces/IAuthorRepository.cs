using LibraryManager.API.Models;

namespace LibraryManager.API.Interfaces
{
    public interface IAuthorRepository
    {
        List<Author> GetAllSync(bool withNoTracking = true, string[]? includes = null);
        Task<List<Author>> GetAllAsync(bool withNoTracking = true, string[]? includes = null, CancellationToken cancellationToken = default);

        Author? GetByIdSync(int id, string[]? includes = null);
        Task<Author?> GetByIdAsync(int id, string[]? includes = null, CancellationToken cancellationToken = default);

        Task AddAsync(Author author, CancellationToken cancellationToken = default);
        Task UpdateAsync(Author author, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}

using LibraryManager.API.DTOs;

namespace LibraryManager.API.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync(bool includeBooks, CancellationToken cancellationToken = default);
        Task<AuthorDto?> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<AuthorDto> CreateAuthorAsync(AuthorDtoCreate data, CancellationToken cancellationToken = default);
        Task UpdateAuthorAsync(int id, AuthorDto data, CancellationToken cancellationToken = default);
        Task DeleteAuthorAsync(int id, CancellationToken cancellationToken = default);
    }
}

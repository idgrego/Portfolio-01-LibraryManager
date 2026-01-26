using LibraryManager.API.DTOs;

namespace LibraryManager.API.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync(bool includeAuthor, CancellationToken cancellationToken = default);
        Task<BookDto?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<BookDto> CreateBookAsync(BookDtoCreate data, CancellationToken cancellationToken = default);
        Task UpdateBookAsync(int id, BookDto data, CancellationToken cancellationToken = default);
        Task DeleteBookAsync(int id, CancellationToken cancellationToken = default);
    }
}

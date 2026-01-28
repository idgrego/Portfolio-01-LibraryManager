using LibraryManager.API.DTOs;
using LibraryManager.API.Exceptions;
using LibraryManager.API.Interfaces;
using LibraryManager.API.Models;

namespace LibraryManager.API.Services
{
    public class BookService : IBookService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        public BookService(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            this._authorRepository = authorRepository;
            this._bookRepository = bookRepository;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync(bool includeAuthor, CancellationToken cancellationToken = default)
        {
            var includes = includeAuthor ? new[] { "Author" } : null;
            var books = await this._bookRepository.GetAllAsync(includes: includes);
            var dtos = books.Select(i => new BookDto
            {
                Id = i.Id,
                Title = i.Title,
                ISBN = i.ISBN,
                PublishedDate = i.PublishedDate,
                AuthorId = i.AuthorId,
                AuthorName = i.Author?.Name ?? (includeAuthor ? "Autor não informado" : string.Empty)
            });
            return dtos;
        }

        public async Task<BookDto?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var book = await this._bookRepository.GetByIdAsync(id, new[] { "Author" }, cancellationToken);

            if (book == null) throw new NotFoundException(nameof(Book), id.ToString());

            var dto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                PublishedDate = book.PublishedDate,
                AuthorId = book.AuthorId,
                AuthorName = book.Author?.Name ?? "Autor não informado"
            };

            return dto;
        }

        public async Task<BookDto> CreateBookAsync(BookDtoCreate data, CancellationToken cancellationToken = default)
        {
            var author = await this._authorRepository.GetByIdAsync(data.AuthorId, null, cancellationToken);
            if (author == null) throw new BadRequestException("Autor informado não existe");

            var book = new Book
            {
                Title = data.Title,
                ISBN = data.ISBN,
                PublishedDate = data.PublishedDate,
                AuthorId = data.AuthorId,
            };

            await this._bookRepository.AddAsync(book);

            var dto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                PublishedDate = book.PublishedDate,
                AuthorId = book.AuthorId,
                AuthorName = author.Name
            };

            return dto;
        }


        public async Task UpdateBookAsync(int id, BookDto data, CancellationToken cancellationToken = default)
        {
            if (id != data.Id)
                throw new BadRequestException("O ID no corpo de requisição não coincide com o ID da URL.");

            var book = await this._bookRepository.GetByIdAsync(id);
            if (book == null) throw new NotFoundException(nameof(Book), id.ToString());

            // se o autor mudou, o novo autor existe
            if (book.AuthorId != data.AuthorId)
            {
                var newAuthor = await this._authorRepository.GetByIdAsync(data.AuthorId);
                if (newAuthor == null) throw new BadRequestException("O novo autor não existe");
            }

            book.Title = data.Title;
            book.ISBN = data.ISBN;
            book.AuthorId = data.AuthorId;
            book.PublishedDate = data.PublishedDate;
            await this._bookRepository.UpdateAsync(book);
        }

        public async Task DeleteBookAsync(int id, CancellationToken cancellationToken = default)
        {
            var book = await this._bookRepository.GetByIdAsync(id);
            if (book == null) throw new NotFoundException(nameof(Book), id.ToString());
            await this._bookRepository.DeleteAsync(id);
        }
    }
}

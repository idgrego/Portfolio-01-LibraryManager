using LibraryManager.API.DTOs;
using LibraryManager.API.Exceptions;
using LibraryManager.API.Interfaces;
using LibraryManager.API.Models;

namespace LibraryManager.API.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorService(IAuthorRepository authorRepository)
        {
            this._authorRepository = authorRepository;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync(bool includeBooks, CancellationToken cancellationToken = default)
        {
            var includes = includeBooks ? new[] { "Books" } : null;
            var authors = await this._authorRepository.GetAllAsync(true, includes, cancellationToken);
            var dtos = authors.Select(i => new AuthorDto
            {
                Id = i.Id,
                Name = i.Name,
                Books = i.Books?.Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN
                }).ToList() ?? new List<BookDto>()
            });
            return dtos;
        }

        public async Task<AuthorDto?> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var author = await this._authorRepository.GetByIdAsync(id, new[] { "Books" }, cancellationToken);

            if (author == null) throw new NotFoundException(nameof(Author), id.ToString());

            var dto = new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Books = author.Books?.Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN
                }).ToList() ?? new List<BookDto>()
            };

            return dto;
        }

        public async Task<AuthorDto> CreateAuthorAsync(AuthorDtoCreate data, CancellationToken cancellationToken = default)
        {
            var author = new Author { Name = data.Name };
            await this._authorRepository.AddAsync(author, cancellationToken);
            var dto = new AuthorDto
            {
                Id = author.Id,
                Name = author.Name
            };
            return dto;
        }


        public async Task UpdateAuthorAsync(int id, AuthorDto data, CancellationToken cancellationToken = default)
        {
            if (id != data.Id)
                throw new BadRequestException("O ID no corpo de requisição não coincide com o ID da URL.");

            var author = await this._authorRepository.GetByIdAsync(id);
            if (author == null) throw new NotFoundException(nameof(Author), id.ToString());

            author.Name = data.Name;
            await this._authorRepository.UpdateAsync(author, cancellationToken);
        }

        public async Task DeleteAuthorAsync(int id, CancellationToken cancellationToken = default)
        {
            var author = await this._authorRepository.GetByIdAsync(id, null, cancellationToken);
            if (author == null) throw new NotFoundException(nameof(Author), id.ToString());
            await this._authorRepository.DeleteAsync(id, cancellationToken);
        }
    }
}

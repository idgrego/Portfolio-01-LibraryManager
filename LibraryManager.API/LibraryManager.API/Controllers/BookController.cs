using LibraryManager.API.DTOs;
using LibraryManager.API.Interfaces;
using LibraryManager.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BookController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;

        public BookController(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            this._authorRepository = authorRepository;
            this._bookRepository = bookRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookDto[]))]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery] bool includeAuthor = false) {
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
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookDto[]))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Book?>> GetBook([FromRoute]int id) { 
            var book = await this._bookRepository.GetByIdAsync(id, new[] { "Author" });

            if (book == null) return NotFound();

            var dto = new BookDto
            { 
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                PublishedDate = book.PublishedDate,
                AuthorId = book.AuthorId,
                AuthorName = book.Author?.Name ?? "Autor não informado"
            };

            return Ok(dto);
        }

        [HttpPost]
        [Consumes(typeof(BookDtoCreate), "application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> CreateBook([FromBody] BookDtoCreate data) {

            var author = await this._authorRepository.GetByIdAsync(data.AuthorId);
            if (author == null) return BadRequest("Autor informado não existe");

            var book = new Book { 
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
            
            // CreatedAtAction: No POST, retornamos o status 201 Created e incluímos no cabeçalho da resposta o link para buscar o autor recém-criado
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, dto);
        }

        [HttpPut("{id}")]
        [Consumes(typeof(BookDto), "application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBook([FromRoute]int id, [FromBody]BookDto data)
        {
            if (id != data.Id)
                return BadRequest("O ID no corpo de requisição não coincide com o ID da URL.");

            var book = await this._bookRepository.GetByIdAsync(id);
            if (book == null) return NotFound();

            // se o autor mudou, o novo autor existe
            if (book.AuthorId != data.AuthorId)
            {
                var newAuthor = await this._authorRepository.GetByIdAsync(data.AuthorId);
                if (newAuthor == null) return BadRequest("O novo autor não existe");
            }

            book.Title = data.Title;
            book.ISBN = data.ISBN;
            book.AuthorId = data.AuthorId;
            book.PublishedDate = data.PublishedDate;
            await this._bookRepository.UpdateAsync(book); 

            return NoContent(); // Status 204: Sucesso, mas não há conteúdo para retornar
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBook([FromRoute]int id)
        {
            var book = await this._bookRepository.GetByIdAsync(id);
            if (book == null) return NotFound();

            await this._bookRepository.DeleteAsync(id); 
            return NoContent();
        }

    }
}

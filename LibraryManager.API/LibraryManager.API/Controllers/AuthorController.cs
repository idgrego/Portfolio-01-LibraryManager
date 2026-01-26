using LibraryManager.API.DTOs;
using LibraryManager.API.Interfaces;
using LibraryManager.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibraryManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")] // Define que o retorno será JSON
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorController(IAuthorRepository authorRepository)
        {
            this._authorRepository = authorRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto[]))]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors([FromQuery] bool includeBooks = false, CancellationToken cancellationToken = default)
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
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorDto>> GetAuthor([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var author = await this._authorRepository.GetByIdAsync(id, new[] { "Books" }, cancellationToken);

            if (author == null) return NotFound();

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

            return Ok(dto);
        }

        [HttpPost]
        [Consumes(typeof(AuthorDtoCreate), "application/json")] // Define que o corpo da requisição deve ser JSON
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthorDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthorDto>> CreateAuthor([FromBody] AuthorDtoCreate data, CancellationToken cancellationToken = default) {
            var author = new Author { Name = data.Name };
            await this._authorRepository.AddAsync(author, cancellationToken);
            var dto = new AuthorDto {
                Id = author.Id,
                Name = author.Name
            };
            
            // CreatedAtAction: No POST, retornamos o status 201 Created e incluímos no cabeçalho da resposta o link para buscar o autor recém-criado
            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, dto);
        }

        [HttpPut("{id}")]
        [Consumes(typeof(AuthorDto), "application/json")] // Define que o corpo da requisição deve ser JSON
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAuthor([FromRoute]int id, [FromBody]AuthorDto data, CancellationToken cancellationToken = default)
        {
            if (id != data.Id)
                return BadRequest("O ID no corpo de requisição não coincide com o ID da URL.");

            var author = await this._authorRepository.GetByIdAsync(id);
            if (author == null) return NotFound();

            author.Name = data.Name;
            await this._authorRepository.UpdateAsync(author, cancellationToken); 

            return NoContent(); // Status 204: Sucesso, mas não há conteúdo para retornar
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAuthor([FromRoute]int id, CancellationToken cancellationToken = default)
        {
            var author = await this._authorRepository.GetByIdAsync(id, null, cancellationToken);
            if (author == null) return NotFound();

            await this._authorRepository.DeleteAsync(id, cancellationToken); 
            return NoContent();
        }
    }
}

using LibraryManager.API.DTOs;
using LibraryManager.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")] // Define que o retorno será JSON
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        public AuthorController(IAuthorService authorService)
        {
            this._authorService = authorService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto[]))]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors([FromQuery] bool includeBooks = false, CancellationToken cancellationToken = default)
        {
            var dtos = await this._authorService.GetAllAuthorsAsync(true, cancellationToken);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorDto>> GetAuthor([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var dto = await this._authorService.GetAuthorByIdAsync(id, cancellationToken);
            return Ok(dto);
        }

        [HttpPost]
        [Consumes(typeof(AuthorDtoCreate), "application/json")] // Define que o corpo da requisição deve ser JSON
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthorDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthorDto>> CreateAuthor([FromBody] AuthorDtoCreate data, CancellationToken cancellationToken = default) {
            var dto = await this._authorService.CreateAuthorAsync(data, cancellationToken);
            return CreatedAtAction(nameof(GetAuthor), new { id = dto.Id }, dto);
            // CreatedAtAction: No POST, retornamos o status 201 Created e incluímos no cabeçalho da resposta o link para buscar o autor recém-criado
        }

        [HttpPut("{id}")]
        [Consumes(typeof(AuthorDto), "application/json")] // Define que o corpo da requisição deve ser JSON
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAuthor([FromRoute]int id, [FromBody]AuthorDto data, CancellationToken cancellationToken = default)
        {
            await this._authorService.UpdateAuthorAsync(id, data, cancellationToken); 
            return NoContent(); // Status 204: Sucesso, mas não há conteúdo para retornar
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAuthor([FromRoute]int id, CancellationToken cancellationToken = default)
        {
            await this._authorService.DeleteAuthorAsync(id, cancellationToken); 
            return NoContent();
        }
    }
}

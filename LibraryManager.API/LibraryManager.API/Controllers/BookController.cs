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
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            this._bookService = bookService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookDto[]))]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery] bool includeAuthor = false, CancellationToken canacellation = default) {
            var dtos = await this._bookService.GetAllBooksAsync(includeAuthor, canacellation);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookDto[]))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Book?>> GetBook([FromRoute]int id, CancellationToken canacellation = default) { 
            var dto = await this._bookService.GetBookByIdAsync(id, canacellation);
            return Ok(dto);
        }

        [HttpPost]
        [Consumes(typeof(BookDtoCreate), "application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> CreateBook([FromBody] BookDtoCreate data, CancellationToken canacellation = default) {
            var dto = await this._bookService.CreateBookAsync(data, canacellation);
            return CreatedAtAction(nameof(GetBook), new { id = dto.Id }, dto);
            // CreatedAtAction: No POST, retornamos o status 201 Created e incluímos no cabeçalho da resposta o link para buscar o autor recém-criado
        }

        [HttpPut("{id}")]
        [Consumes(typeof(BookDto), "application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBook([FromRoute]int id, [FromBody]BookDto data, CancellationToken canacellation = default)
        {
            await this._bookService.UpdateBookAsync(id, data, canacellation); 
            return NoContent(); // Status 204: Sucesso, mas não há conteúdo para retornar
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBook([FromRoute]int id, CancellationToken canacellation = default)
        {
            await this._bookService.DeleteBookAsync(id, canacellation); 
            return NoContent();
        }

    }
}

using FluentAssertions;
using LibraryManager.API.DTOs;
using LibraryManager.API.Exceptions;
using LibraryManager.API.Interfaces;
using LibraryManager.API.Models;
using LibraryManager.API.Services;
using Moq;

namespace LibraryManager.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IAuthorRepository> _authorRepoMock;
    //private readonly AuthorService _authorService;

    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        // dublê do repositório
        _authorRepoMock = new Mock<IAuthorRepository>();
        //_authorService = new AuthorService(_authorRepoMock.Object);

        _bookRepoMock = new Mock<IBookRepository>();
        _bookService = new BookService(_authorRepoMock.Object, _bookRepoMock.Object);
    }

    [Fact]
    public async Task GetBookById_WhenBookExists_ShouldReturnBookDto()
    {
        // Arrange (preparar)
        var author = new Author { Id = 1, Name = "J.K. Rowling", Books = null };
        var book = new Book { Id = 1, AuthorId = author.Id, ISBN = "1234567890123", PublishedDate = new DateTime(2026, 1, 28), Title = "Lord of the Rings", Author = author };
        _bookRepoMock.Setup(repo => repo.GetByIdAsync(book.Id, It.IsAny<string[]>(), It.IsAny<CancellationToken>())).ReturnsAsync(book);

        // Act (agir)
        var result = await _bookService.GetBookByIdAsync(book.Id, default);

        // Assert (verificar)
        result.Should().NotBeNull();
        result.Title.Should().Be(book.Title);
        result.Id.Should().Be(book.Id);
        result.AuthorId.Should().Be(book.AuthorId);
    }

    [Fact]
    public async Task GetBookById_WhenAuthorDoesNotExists_ShouldThrowNotFoundException()
    {
        // Arrange (preparar)
        var bookId = 99;
        _bookRepoMock.Setup(repo => repo.GetByIdAsync(bookId, null, default)).ReturnsAsync((Book?)null);

        // Act (agir) & Assert (verificar)
        // a instrução a seguir funciona e faz o mesmo que o try-catch
        // await _bookService.Invoking(s => s.GetBookByIdAsync(bookId)).Should().ThrowAsync<NotFoundException>();

        try
        {
            // Act (agir)
            await _bookService.GetBookByIdAsync(bookId);
        }
        catch (NotFoundException ex)
        {
            // Assert (verificar)
            ex.Should().BeOfType<NotFoundException>();
        }

    }

    [Fact]
    public async Task CreateBook_WhenBookDoesNotExists_ShouldReturnBookDto()
    {
        /*
         * === O Problema da Instância (Strict Matching):
         * Quando você faz _repositoryMock.Setup(repo => repo.AddAsync(author, ...)), o Moq entende o seguinte: 
         * "Só responda Sucesso se o método for chamado com exatamente este objeto author que criei aqui no teste".
         * 
         * Porém, dentro do seu AuthorService, você provavelmente faz um new Author { Name = dto.Name }. 
         * Mesmo que os nomes sejam iguais, para o C# são dois objetos diferentes na memória. 
         * O Moq não vai reconhecer a chamada e vai retornar null (ou erro), fazendo o teste falhar.
         * 
         * === A solução:
         * A Forma Correta (Usando It.IsAny ou It.Is)
         * Para o teste funcionar, você deve dizer ao Moq para aceitar qualquer objeto do tipo Author, ou um que tenha o nome correto
         * 
         * === Observações
         * 1) ReturnsAsync(): Se o seu método no repositório for Task AddAsync(...) (sem retorno), o correto no Moq é .Returns(Task.CompletedTask). 
         * Se ele retornar o objeto criado, ex: Task<Author> AddAsync(...), aí sim você usa .ReturnsAsync(author).
         * 
         * 2) CancellationToken: Como você está usando CancellationToken no repositório, no setup você deve usar It.IsAny<CancellationToken>() 
         * para que o Moq ignore qual token está sendo passado.
         */

        // Arrange
        var author = new Author { Id = 1, Name = "J.K. Rowling", Books = null };
        var dtoCreate = new BookDtoCreate { AuthorId = 1, Title = "Lord of the Rings", ISBN = "1234567890123", PublishedDate = new DateTime(2026, 1, 28) };

        // Prepara o repositório do Author
        _authorRepoMock.Setup(repo => repo.GetByIdAsync(author.Id, It.IsAny<string[]>(), It.IsAny<CancellationToken>())).ReturnsAsync(author);

        // Opção A: Aceitar qualquer Livro (Mais comum)
        _bookRepoMock.Setup(repo => repo.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask); // Se o método retornar Task

        // Opção B: Validar se o nome está correto (Mais rigoroso)
        _bookRepoMock.Setup(repo => repo.AddAsync(It.Is<Book>(a => a.Title == dtoCreate.Title), It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

        // Act
        var result = await _bookService.CreateBookAsync(dtoCreate);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(dtoCreate.Title);

        // Verifica se o repositório foi realmente acionado
        _bookRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBook_WhenBookExists_ShouldThrowException()
    {
        // Arrange
        var author = new Author { Id = 1, Name = "J.K. Rowling", Books = null };
        var dtoCreate = new BookDtoCreate { AuthorId = 1, Title = "Lord of the Rings", ISBN = "1234567890123", PublishedDate = new DateTime(2026, 1, 28) };

        // Prepara o repositório do Author
        _authorRepoMock.Setup(repo => repo.GetByIdAsync(author.Id, null, It.IsAny<CancellationToken>())).ReturnsAsync(author);

        // Validar se o nome está correto (Mais rigoroso)
        _bookRepoMock.Setup(repo => repo.AddAsync(It.Is<Book>(a => a.Title == dtoCreate.Title), It.IsAny<CancellationToken>()))
                       .Throws<Exception>();

        // Act & Assert
        await _bookService.Invoking(s => s.CreateBookAsync(dtoCreate)).Should().ThrowAsync<Exception>();

        // Verifica se o repositório foi realmente acionado
        _bookRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBook_WhenIDMismatch_ShouldThrowBadRequestException()
    {
        // Arrange (preparar)
        var bookId = 99;
        var dtoUpdate = new BookDto { Id = 1, Title = "J.K. Rowling", AuthorId = 1, ISBN = "1234567890123", PublishedDate = new DateTime(2026,1,28) };

        // Act (agir) & Assert (verificar)
        await _bookService.Invoking(s => s.UpdateBookAsync(bookId, dtoUpdate))
            .Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task UpdateBook_WhenAuthorExists_ShouldUpdateTitle()
    {
        // Arrange (preparar)
        var bookId = 1;
        var dtoUpdate = new BookDto { Id = 1, Title = "Lord of the Rings", AuthorId = 1, ISBN = "1234567890123", PublishedDate = new DateTime(2026,1,28) };
        var dtoExists = new Book { Id = 1, Title = "Lord Rings", AuthorId = 1, ISBN = "1234567890123", PublishedDate = new DateTime(2026, 1, 28) };
        _bookRepoMock.Setup(repo => repo.GetByIdAsync(bookId, null, It.IsAny<CancellationToken>())).ReturnsAsync(dtoExists);

        // Act (agir) & Assert (verificar)
        await _bookService.UpdateBookAsync(bookId, dtoUpdate);

        // Assert (verificar)
        _bookRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
        dtoExists.Should().NotBeNull();
        dtoExists.Title.Should().Be(dtoUpdate.Title);

    }

    [Fact]
    public async Task DeleteBook_WhenBookExists_ShouldCallDelete()
    {
        // Arrange (preparar)
        var bookId = 1;
        var dtoExists = new Book { Id = 1, Title = "Lord Rings", AuthorId = 1, ISBN = "1234567890123", PublishedDate = new DateTime(2026, 1, 28) };

        _bookRepoMock.Setup(repo => repo.GetByIdAsync(bookId, null, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(dtoExists);

        // Act (agir) & Assert (verificar)
        await _bookService.DeleteBookAsync(bookId);

        // Assert (verificar)
        _bookRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);

    }
}

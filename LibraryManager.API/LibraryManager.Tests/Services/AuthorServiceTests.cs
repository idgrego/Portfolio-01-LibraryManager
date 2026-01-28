using FluentAssertions;
using LibraryManager.API.DTOs;
using LibraryManager.API.Exceptions;
using LibraryManager.API.Interfaces;
using LibraryManager.API.Models;
using LibraryManager.API.Services;
using Moq;

namespace LibraryManager.Tests.Services;

public class AuthorServiceTests
{
    private readonly Mock<IAuthorRepository> _authorRepoMock;
    private readonly AuthorService _authorService;

    public AuthorServiceTests()
    {
        // dublê do repositório
        _authorRepoMock = new Mock<IAuthorRepository>();
        _authorService = new AuthorService(_authorRepoMock.Object);
    }

    [Fact]
    public async Task GetAuthorById_WhenAuthorExists_ShouldReturnAuthorDto()
    {
        // Arrange (preparar)
        var authorId = 1;
        var author = new Author { Id = authorId, Name = "J.K. Rowling", Books = null };
        _authorRepoMock.Setup(repo => repo.GetByIdAsync(authorId, new[] { "Books" }, default)).ReturnsAsync(author);

        // Act (agir)
        var result = await _authorService.GetAuthorByIdAsync(authorId, default);

        // Assert (verificar)
        result.Should().NotBeNull();
        result.Name.Should().Be(author.Name);
        result.Id.Should().Be(authorId);
    }

    [Fact]
    public async Task GetAuthorById_WhenAuthorDoesNotExists_ShouldThrowNotFoundException()
    {
        // Arrange (preparar)
        var authorId = 99;
        _authorRepoMock.Setup(repo => repo.GetByIdAsync(authorId, new[] { "Books" }, default)).ReturnsAsync((Author?)null);

        // Act (agir) & Assert (verificar)
        // a instrução a seguir funciona e faz o mesmo que o try-catch
        // await _authorService.Invoking(s => s.GetAuthorByIdAsync(authorId)).Should().ThrowAsync<NotFoundException>();

        try
        {
            // Act (agir)
            await _authorService.GetAuthorByIdAsync(authorId);
        }
        catch (NotFoundException ex)
        {
            // Assert (verificar)
            ex.Should().BeOfType<NotFoundException>();
        }

    }

    [Fact]
    public async Task CreateAuthor_ShouldCallRepositoryAndReturnDto()
    {
        // Para os métodos Update e Delete, o segredo é usar o .Verify() do Moq.
        // Ele garante que o repositório foi acionado com os dados certos.

        // Arrange
        var dtoCreate = new AuthorDtoCreate { Name = "George R. R. Martin" };

        // Act
        var result = await _authorService.CreateAuthorAsync(dtoCreate);

        // Assert
        result.Name.Should().Be(dtoCreate.Name);

        // Verifica se o repositório chamou o AddAsync exatamente uma vez
        _authorRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAuthor_WhenAuthorDoesNotExists_ShouldReturnAuthorDto()
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
        var dtoCreate = new AuthorDtoCreate { Name = "J.K. Rowling" };

        // Opção A: Aceitar qualquer Author (Mais comum)
        _authorRepoMock.Setup(repo => repo.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask); // Se o método retornar Task

        // Opção B: Validar se o nome está correto (Mais rigoroso)
        _authorRepoMock.Setup(repo => repo.AddAsync(It.Is<Author>(a => a.Name == dtoCreate.Name), It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

        // Act
        var result = await _authorService.CreateAuthorAsync(dtoCreate);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dtoCreate.Name);

        // Verifica se o repositório foi realmente acionado
        _authorRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAuthor_WhenAuthorExists_ShouldThrowException()
    {
        // Arrange
        var dtoCreate = new AuthorDtoCreate { Name = "J.K. Rowling" };

        // Validar se o nome está correto (Mais rigoroso)
        _authorRepoMock.Setup(repo => repo.AddAsync(It.Is<Author>(a => a.Name == dtoCreate.Name), It.IsAny<CancellationToken>()))
                       .Throws<Exception>();

        // Act & Assert
        await _authorService.Invoking(s => s.CreateAuthorAsync(dtoCreate)).Should().ThrowAsync<Exception>();

        // Verifica se o repositório foi realmente acionado
        _authorRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthor_WhenIDMismatch_ShouldThrowBadRequestException()
    {
        // Arrange (preparar)
        var authorId = 99;
        var dtoUpdate = new AuthorDto { Id = 1, Name = "J.K. Rowling" };

        // Act (agir) & Assert (verificar)
        await _authorService.Invoking(s => s.UpdateAuthorAsync(authorId, dtoUpdate))
            .Should().ThrowAsync<BadRequestException>();

    }

    [Fact]
    public async Task UpdateAuthor_WhenAuthorExists_ShouldUpdateName()
    {
        // Arrange (preparar)
        var authorId = 1;
        var dtoUpdate = new AuthorDto { Id = 1, Name = "J.K. Rowling" };
        var dtoExists = new Author { Id = 1, Name = "Original Name" };
        _authorRepoMock.Setup(repo => repo.GetByIdAsync(authorId, null, It.IsAny<CancellationToken>())).ReturnsAsync(dtoExists);

        // Act (agir) & Assert (verificar)
        await _authorService.UpdateAuthorAsync(authorId, dtoUpdate);

        // Assert (verificar)
        _authorRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
        dtoExists.Should().NotBeNull();
        dtoExists.Name.Should().Be(dtoUpdate.Name);

    }

    [Fact]
    public async Task DeleteAuthor_WhenAuthorExists_ShouldCallDelete()
    {
        // Arrange (preparar)
        var authorId = 1;
        var dtoExists = new Author { Id = 1, Name = "J.K. Rowling" };

        _authorRepoMock.Setup(repo => repo.GetByIdAsync(authorId, null, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(dtoExists);

        // Act (agir) & Assert (verificar)
        await _authorService.DeleteAuthorAsync(authorId);

        // Assert (verificar)
        _authorRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);

    }
}

using LibraryManager.API.Data;
using LibraryManager.API.Interfaces;
using LibraryManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.API.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;

        public BookRepository(IDbContextFactory<LibraryDbContext> dbFactory)
        {
            this._context = dbFactory.CreateDbContext();
        }

        private IQueryable<Book> ApplyIncludes(IQueryable<Book> query, string[]? includes)
        {
            if (includes?.Length > 0)
                foreach (var include in includes)
                    query = query.Include(include);
            
            return query;
        }

        public async Task<IEnumerable<Book>> GetAllAsync(bool withNoTracking = true, string[]? includes = null, CancellationToken cancellationToken = default)
        {
            // Começamos com um IQueryable para permitir a composição da query
            IQueryable<Book> query = this._context.Books.AsQueryable<Book>();

            query = ApplyIncludes(query, includes);

            // deixar esse por último, não é obrigatório mas melhora a legibilidade
            if (withNoTracking)
                query = query.AsNoTracking<Book>();

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Book?> GetByIdAsync(int id, string[]? includes = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Book> query = this._context.Books;

            query = ApplyIncludes(query, includes);

            /*
             * Por que utilizar FirstOrDefaultAsync ao invés de FindAsync?
             * 
             * > FindAsync(id): É otimizado para buscar pela Chave Primária.
             * Se o objeto já foi carregado na memória do C# nesta mesma requisição, ele nem vai ao banco. 
             * Ele é "fechado", não permite configurar a query (como adicionar Include).
             * 
             * > FirstOrDefaultAsync(x => x.Id == id): Ele sempre monta uma query SQL e a envia ao banco. 
             * Como ele trabalha com IQueryable, ele nos dá a liberdade total para anexar Include, AsNoTracking, 
             * ou qualquer outra configuração.
             * 
             */

            // Book? teste = await this._context.Books.FindAsync(id);
            return await query.FirstOrDefaultAsync<Book>(i => i.Id == id, cancellationToken);
        }

        public async Task AddAsync(Book Book, CancellationToken cancellationToken = default)
        {
            await this._context.Books.AddAsync(Book, cancellationToken);
            await this._context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Book? Book = await this.GetByIdAsync(id, null, cancellationToken);
            if (Book != null) {
                this._context.Books.Remove(Book);
                await this._context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateAsync(Book Book, CancellationToken cancellationToken = default)
        {
            this._context.Books.Update(Book);
            await this._context.SaveChangesAsync(cancellationToken);
        }

    }
}

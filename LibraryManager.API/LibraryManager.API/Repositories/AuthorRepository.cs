using LibraryManager.API.Data;
using LibraryManager.API.Interfaces;
using LibraryManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.API.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryDbContext _context;

        public AuthorRepository(IDbContextFactory<LibraryDbContext> dbFactory)
        {
            this._context = dbFactory.CreateDbContext(); 
        }

        private IQueryable<Author> ApplyIncludes(IQueryable<Author> query, string[]? includes)
        {
            if (includes?.Length > 0)
                foreach (var include in includes)
                    query = query.Include(include);
            
            return query;
        }

        public async Task<List<Author>> GetAllAsync(bool withNoTracking = true, string[]? includes = null, CancellationToken cancellationToken = default)
        {
            // Começamos com um IQueryable para permitir a composição da query
            IQueryable<Author> query = this._context.Authors.AsQueryable();

            query = ApplyIncludes(query, includes);

            // deixar esse por último, não é obrigatório mas melhora a legibilidade
            if (withNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Author?> GetByIdAsync(int id, string[]? includes = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Author> query = this._context.Authors;

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

            // Author? teste = await this._context.Authors.FindAsync(id);
            return await query.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
            
        }

        public async Task AddAsync(Author author, CancellationToken cancellationToken)
        {
            await this._context.Authors.AddAsync(author, cancellationToken);
            await this._context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Author? author = await this.GetByIdAsync(id, null, cancellationToken);
            if (author != null) {
                this._context.Authors.Remove(author);
                await this._context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateAsync(Author author, CancellationToken cancellationToken = default)
        {
            this._context.Authors.Update(author);
            await this._context.SaveChangesAsync(cancellationToken);
        }
    }
}

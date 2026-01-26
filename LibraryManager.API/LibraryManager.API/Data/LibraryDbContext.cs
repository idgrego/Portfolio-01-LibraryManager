using LibraryManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.API.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) {}

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // sempre no início
            base.OnModelCreating(modelBuilder);

            // aqui podemos utilizar a Fluent API para configurações finas
            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId);

            // 1. Definindo a "receita" de comparação (Case + Accent Insensitive)
            // 'ks-level1' ignora acentos e case no padrão ICU
            modelBuilder.HasCollation("ignore_case_and_accents",
                locale: "en-u-ks-level1",
                provider: "icu",
                deterministic: false);

            // 2. Aplicando as regras na entidade Author
            modelBuilder.Entity<Author>(entity =>
            {
                entity.Property(a => a.Name)
                    .HasMaxLength(100) // Limite de 100 caracteres
                    .IsRequired()
                    .UseCollation("ignore_case_and_accents"); // Aplica a colação especial

                // 3. Criando o índice único
                entity.HasIndex(a => a.Name)
                    .IsUnique();
            });

            // 3. Aplicando as regras na entidade Book
            modelBuilder.Entity<Book>(entity =>
            {
                // 1. Configuração do Título (Case e Accent Insensitive)
                entity.Property(b => b.Title)
                    .HasMaxLength(200) // Livros costumam ter títulos maiores
                    .IsRequired()
                    .UseCollation("ignore_case_and_accents");

                // 2. Configuração do ISBN
                entity.Property(b => b.ISBN)
                    .HasMaxLength(20)
                    .IsRequired();

                // 3. Índice Único Composto (Título + AutorId)
                // Impede títulos duplicados para o mesmo autor
                entity.HasIndex(b => new { b.Title, b.AuthorId })
                    .IsUnique()
                    .HasDatabaseName("IX_Book_Title_AuthorId_Unique");

                // 4. Índice Único Simples (ISBN)
                // Garante que o mesmo ISBN não seja cadastrado duas vezes no sistema
                entity.HasIndex(b => b.ISBN)
                    .IsUnique()
                    .HasDatabaseName("IX_Book_ISBN_Unique");
            });

        }

        
    }
}

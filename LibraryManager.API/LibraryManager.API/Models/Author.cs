namespace LibraryManager.API.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // um autor pode ter vários livros
        public ICollection<Book>? Books { get; set; }
    }
}

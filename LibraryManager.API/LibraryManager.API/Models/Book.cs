namespace LibraryManager.API.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }

        // Chave estrangeira para Autor
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}
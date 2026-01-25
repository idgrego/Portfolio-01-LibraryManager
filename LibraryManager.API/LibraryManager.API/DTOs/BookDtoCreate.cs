using System.ComponentModel.DataAnnotations;

namespace LibraryManager.API.DTOs
{
    public class BookDtoCreate
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage ="O ISBN é obrigatório")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage ="O ID do autor é obrigatório")]
        public int AuthorId { get; set; }

        public DateTime PublishedDate { get; set; }
    }
}

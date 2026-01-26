using System.ComponentModel.DataAnnotations;

namespace LibraryManager.API.DTOs
{
    public class BookDtoCreate
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage ="O ISBN é obrigatório")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage ="O ID do autor é obrigatório")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Data de publicação é obrigatória")]
        public DateTime PublishedDate { get; set; }
    }
}

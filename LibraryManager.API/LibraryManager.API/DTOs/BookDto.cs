using System.ComponentModel.DataAnnotations;

namespace LibraryManager.API.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres")]
        public string Title { get; set; } = string.Empty;


        [Required(ErrorMessage = "O ISBN é obrigatório")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 20 caracteres")]
        public string ISBN { get; set; } = string.Empty;


        [Required(ErrorMessage = "Data de publicação é obrigatória")]
        public DateTime PublishedDate { get; set; }


        // Campo extraído do relacionamento
        public string AuthorName { get; set; } = string.Empty;


        [Required(ErrorMessage = "O ID do autor é obrigatório")]
        public int AuthorId { get; set; }
    }
}

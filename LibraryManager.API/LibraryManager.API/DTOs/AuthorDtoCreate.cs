using System.ComponentModel.DataAnnotations;

namespace LibraryManager.API.DTOs
{
    public class AuthorDtoCreate
    {
        [Required(ErrorMessage = "O nome do autor é obrigatório")]
        [StringLength(100, MinimumLength=3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        public string Name { get; set; } = string.Empty;
    }
}

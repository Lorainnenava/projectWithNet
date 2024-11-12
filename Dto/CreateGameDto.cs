using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Dto;
public record class CreateGameDto(
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(20, ErrorMessage = "El max de caracteres es 20")]
    string Name,
    int GenreId,
    decimal Price,
    DateOnly ReleaseDate
);

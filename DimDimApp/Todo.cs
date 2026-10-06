using System.ComponentModel.DataAnnotations;

public class Todo
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    public DateTime? DueBy { get; set; }
    public bool IsComplete { get; set; } = false;
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}

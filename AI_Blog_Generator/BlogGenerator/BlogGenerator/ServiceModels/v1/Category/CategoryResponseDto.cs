namespace BlogGenerator.ServiceModels.v1.Category;

public class CategoryResponseDto
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Icon { get; set; }

    public DateTime CreatedAt { get; set; }
}
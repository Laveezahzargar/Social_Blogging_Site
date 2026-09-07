namespace BlogGenerator.ServiceModels.v1.Category;

public class CategoryRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Icon { get; set; }
}
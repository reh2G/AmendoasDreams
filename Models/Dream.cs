namespace AmendoasDreams.Api.Models;

public class Dream
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly Dream_Date { get; set; }
    public TimeOnly? Sleep_Time { get; set; }
    public TimeOnly? Wake_Time { get; set; }
    public string? Mood { get; set; }
    public DateTime Created_At { get; set; }
    public DateTime? Updated_At { get; set; }
    public List<Tag> Tags { get; set; } = [];
}
